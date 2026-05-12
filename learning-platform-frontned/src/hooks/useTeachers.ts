import { useQuery, UseQueryResult } from '@tanstack/react-query'
import type { PagedResult, TeacherCardDto, TeacherLevel } from '../types'
import { teacherService } from '../api/services/teacherService'

export const useTeachers = (params: {
  currency: string
  level?: TeacherLevel
  page?: number
  pageSize?: number
}): UseQueryResult<PagedResult<TeacherCardDto>, Error> => {
  return useQuery({
    queryKey: ['teachers', params],
    queryFn: () => teacherService.getTeachers(params),
    staleTime: 1000 * 60 * 5, // 5 minutes
    retry: 3,
    retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
  })
};

export const useTeacherById = (id: string) => {
  return useQuery({
    queryKey: ['teacher', id],
    queryFn: () => teacherService.getTeacherById(id),
    enabled: !!id,
    staleTime: 1000 * 60 * 10,
    retry: 3,
  })
}
