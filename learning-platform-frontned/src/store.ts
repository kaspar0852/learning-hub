import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface FavoritesStore {
  favorites: string[]
  addFavorite: (teacherId: string) => void
  removeFavorite: (teacherId: string) => void
  setFavorites: (teacherIds: string[]) => void
  isFavorite: (teacherId: string) => boolean
}

export const useFavoritesStore = create<FavoritesStore>()(
  persist(
    (set, get) => ({
      favorites: [],
      addFavorite: (teacherId: string) => {
        const favorites = get().favorites
        if (!favorites.includes(teacherId)) {
          set({ favorites: [...favorites, teacherId] })
        }
      },
      removeFavorite: (teacherId: string) => {
        const favorites = get().favorites
        set({ favorites: favorites.filter(id => id !== teacherId) })
      },
      setFavorites: (teacherIds: string[]) => {
        set({ favorites: teacherIds })
      },
      isFavorite: (teacherId: string) => {
        return get().favorites.includes(teacherId)
      },
    }),
    {
      name: 'favorites-storage',
    }
  )
)
