import apiClient from '../client';
import type {
  CreateTeacherDto,
  PagedResult,
  TeacherCardDto,
  TeacherDto,
  TeacherLevel,
  UpdateTeacherDto,
} from '../../types';
import { getOrCreateSessionId } from '../../lib/session';

export const teacherService = {
  getTeachers: async (params: {
    currency: string;
    level?: TeacherLevel;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResult<TeacherCardDto>> => {
    const sessionId = getOrCreateSessionId();
    const currency = (params.currency ?? '').trim() || 'USD';
    const response = await apiClient.get<PagedResult<TeacherCardDto>>('/api/teachers', {
      params: {
        currency,
        sessionId,
        level: params.level,
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 20,
      },
    });
    return response.data;
  },

  getTeacherById: async (id: string): Promise<TeacherDto> => {
    const response = await apiClient.get<TeacherDto>(`/api/teachers/${id}`);
    return response.data;
  },

  createTeacher: async (dto: CreateTeacherDto): Promise<TeacherDto> => {
    const response = await apiClient.post<TeacherDto>('/api/teachers', dto);
    return response.data;
  },

  updateTeacher: async (teacherId: string, dto: UpdateTeacherDto): Promise<TeacherDto> => {
    const response = await apiClient.put<TeacherDto>(`/api/teachers/${teacherId}`, dto);
    return response.data;
  },

  deleteTeacher: async (teacherId: string): Promise<void> => {
    await apiClient.delete(`/api/teachers/${teacherId}`);
  },
};
