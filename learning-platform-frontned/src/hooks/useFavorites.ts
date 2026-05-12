import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { favoritesService } from '../api/services/favoritesService'
import { useFavoritesStore } from '../store'
import { useEffect } from 'react'

export const useFavorites = () => {
  const queryClient = useQueryClient()
  const { favorites, setFavorites, addFavorite, removeFavorite } = useFavoritesStore()

  const { data, isLoading, isError } = useQuery({
    queryKey: ['favorites'],
    queryFn: () => favoritesService.getFavorites(),
    staleTime: 1000 * 60,
  })

  // Keep persisted store in sync for header badge / offline UI.
  const teacherIds = data?.teacherIds ?? favorites
  useEffect(() => {
    if (data) setFavorites(data.teacherIds)
  }, [data, setFavorites])

  const addMutation = useMutation({
    mutationFn: (teacherId: string) => favoritesService.addFavorite(teacherId),
    onMutate: async (teacherId) => {
      addFavorite(teacherId)
      await queryClient.cancelQueries({ queryKey: ['teachers'] })
    },
    onSettled: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['favorites'] }),
        queryClient.invalidateQueries({ queryKey: ['teachers'] }),
      ])
    },
  })

  const removeMutation = useMutation({
    mutationFn: (teacherId: string) => favoritesService.removeFavorite(teacherId),
    onMutate: async (teacherId) => {
      removeFavorite(teacherId)
      await queryClient.cancelQueries({ queryKey: ['teachers'] })
    },
    onSettled: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['favorites'] }),
        queryClient.invalidateQueries({ queryKey: ['teachers'] }),
      ])
    },
  })

  return {
    favorites: teacherIds,
    isLoading,
    isError,
    addFavorite: (teacherId: string) => addMutation.mutate(teacherId),
    removeFavorite: (teacherId: string) => removeMutation.mutate(teacherId),
    isMutating: addMutation.isPending || removeMutation.isPending,
  }
}
