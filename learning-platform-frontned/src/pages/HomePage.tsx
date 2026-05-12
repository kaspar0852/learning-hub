import { useState, useMemo } from 'react'
import TeacherCard from '../components/TeacherCard'
import FilterBar from '../components/FilterBar'
import { LoadingGrid } from '../components/LoadingGrid'
import { ErrorState } from '../components/states/ErrorState'
import { EmptyState } from '../components/states/EmptyState'
import { useTeachers } from '../hooks/useTeachers'
import { useLocation } from '../hooks/useLocation'
import type { TeacherLevel } from '../types'
import { useFavorites } from '../hooks/useFavorites'

export default function HomePage() {
  const [searchQuery, setSearchQuery] = useState('')
  const [selectedLevel, setSelectedLevel] = useState<TeacherLevel | ''>('')
  const [page, setPage] = useState(1)
  const pageSize = 20
  
  const { location } = useLocation()
  const { addFavorite, removeFavorite } = useFavorites()
  const { data, isLoading, isError, refetch } = useTeachers({
    currency: location.currencyCode,
    level: selectedLevel || undefined,
    page,
    pageSize,
  })
  const teachers = data?.items ?? []

  const filteredTeachers = useMemo(() => {
    return teachers.filter((teacher) => {
      // Search filter
      if (searchQuery && !teacher.name.toLowerCase().includes(searchQuery.toLowerCase())) {
        return false
      }

      return true
    })
  }, [teachers, searchQuery])

  return (
    <main className="bg-background min-h-screen">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Hero Section */}
        <div className="mb-12 text-center">
          <div className="bg-gradient-to-r from-accent/10 to-accent/5 rounded-2xl p-8 mb-8">
            <h2 className="text-5xl font-bold text-foreground mb-4 bg-gradient-to-r from-foreground to-accent bg-clip-text text-transparent">
              Find Your Perfect English Teacher
            </h2>
            <p className="text-lg text-muted-foreground max-w-3xl mx-auto leading-relaxed">
              Connect with experienced teachers from around the world. Learn at your own pace with personalized lessons tailored to your goals and schedule.
            </p>
          </div>
        </div>

        {/* Filter Bar */}
        <FilterBar
          searchValue={searchQuery}
          levelValue={selectedLevel ? String(selectedLevel) : ''}
          onSearchChange={setSearchQuery}
          onLevelChange={(value) => setSelectedLevel(value ? value as TeacherLevel : '')}
        />

        {/* Results Info */}
        <div className="mb-6 bg-muted/50 rounded-lg px-4 py-3 flex items-center justify-between">
          <p className="text-sm text-muted-foreground">
            Showing <span className="font-semibold text-foreground">{filteredTeachers.length}</span> teachers
            {searchQuery && ` matching "${searchQuery}"`}
          </p>
          {filteredTeachers.length > 0 && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <div className="w-2 h-2 bg-green-500 rounded-full"></div>
              <span>All available now</span>
            </div>
          )}
        </div>

        {/* Teachers Grid */}
        {isLoading ? (
          <LoadingGrid count={6} />
        ) : isError ? (
          <ErrorState
            title="Failed to load teachers"
            message="We couldn't load the teachers list. Please check your connection and try again."
            onRetry={() => refetch()}
            showRetry={true}
          />
        ) : filteredTeachers.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {filteredTeachers.map((teacher) => (
              <TeacherCard
                key={teacher.id}
                teacher={teacher}
                onToggleFavorite={(teacherId, isFavorite) =>
                  isFavorite ? removeFavorite(teacherId) : addFavorite(teacherId)
                }
              />
            ))}
          </div>
        ) : (
          <EmptyState
            title="No teachers found"
            message="Try adjusting your filters or search query"
            action={
              <button
                onClick={() => {
                  setSearchQuery('')
                  setSelectedLevel('')
                }}
                className="text-accent hover:underline font-medium"
              >
                Clear all filters
              </button>
            }
          />
        )}

        {/* Paging */}
        {data && data.totalCount > data.pageSize && (
          <div className="mt-12 flex items-center justify-center gap-4">
            <button
              className="px-6 py-3 bg-card border border-border rounded-lg hover:bg-accent hover:text-accent-foreground disabled:opacity-50 disabled:cursor-not-allowed transition-all font-medium shadow-sm hover:shadow-md"
              disabled={page <= 1}
              onClick={() => setPage((p) => Math.max(1, p - 1))}
            >
              ← Previous
            </button>
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium text-foreground">Page</span>
              <span className="px-3 py-1 bg-accent text-accent-foreground rounded-md font-semibold">{data.page}</span>
              <span className="text-sm text-muted-foreground">of {Math.ceil(data.totalCount / data.pageSize)}</span>
            </div>
            <button
              className="px-6 py-3 bg-card border border-border rounded-lg hover:bg-accent hover:text-accent-foreground disabled:opacity-50 disabled:cursor-not-allowed transition-all font-medium shadow-sm hover:shadow-md"
              disabled={page >= Math.ceil(data.totalCount / data.pageSize)}
              onClick={() => setPage((p) => p + 1)}
            >
              Next →
            </button>
          </div>
        )}
      </div>
    </main>
  )
}
