// Matches backend enum values: "Beginner" | "Advanced"
export type TeacherLevel = 'Beginner' | 'Advanced'

export const TEACHER_LEVEL_LABEL: Record<TeacherLevel, string> = {
  Beginner: 'Beginner',
  Advanced: 'Advanced',
}

export interface TeacherCardDto {
  id: string
  name: string
  level: TeacherLevel
  basePriceUsd: number
  localizedPrice: number
  currency: string
  isFavorite: boolean
}

export interface TeacherDto {
  id: string
  name: string
  level: TeacherLevel
  basePriceUsd: number
}

export interface CreateTeacherDto {
  name: string
  level: TeacherLevel
  basePriceUsd: number
}

export interface UpdateTeacherDto {
  name: string
  level: TeacherLevel
  basePriceUsd: number
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export interface CurrencyDetectionDto {
  currencyCode: string
  countryCode?: string | null
  countryName?: string | null
}
