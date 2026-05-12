import apiClient from '../client'
import { getOrCreateSessionId } from '../../lib/session'

export interface FavoritesDto {
  sessionId: string
  teacherIds: string[]
}

export const favoritesService = {
  getFavorites: async (): Promise<FavoritesDto> => {
    const sessionId = getOrCreateSessionId()
    const response = await apiClient.get<FavoritesDto>(`/api/sessions/${sessionId}/favorites`)
    return response.data
  },

  addFavorite: async (teacherId: string): Promise<void> => {
    const sessionId = getOrCreateSessionId()
    await apiClient.post(`/api/sessions/${sessionId}/favorites/${teacherId}`)
  },

  removeFavorite: async (teacherId: string): Promise<void> => {
    const sessionId = getOrCreateSessionId()
    await apiClient.delete(`/api/sessions/${sessionId}/favorites/${teacherId}`)
  },
}

