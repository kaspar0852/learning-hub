import { Link } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'
import TeacherCard from '../components/TeacherCard'
import { EmptyState } from '../components/states/EmptyState'
import { useFavorites } from '../hooks/useFavorites'
import { useLocation } from '../hooks/useLocation'
import { Button } from '../components/ui/button'
import { useTeachers } from '../hooks/useTeachers'

export default function FavoritesPage() {
  const { favorites, addFavorite, removeFavorite } = useFavorites()
  const { location } = useLocation()
  const { data, isLoading, isError } = useTeachers({
    currency: location.currencyCode,
    page: 1,
    pageSize: 100,
  })
  const favoriteTeachers = (data?.items ?? []).filter((t) => t.isFavorite || favorites.includes(t.id))

  return (
    <main className="bg-background min-h-screen">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <Link
            to="/"
            className="inline-flex items-center gap-2 text-accent hover:text-accent/80 transition-colors mb-6"
          >
            <ArrowLeft className="w-4 h-4" />
            Back to all teachers
          </Link>
          
          <h2 className="text-4xl font-bold text-foreground mb-2">
            Your Favorite Teachers
          </h2>
          <p className="text-lg text-muted-foreground">
            {favoriteTeachers.length} teacher{favoriteTeachers.length !== 1 ? 's' : ''} saved
          </p>
        </div>

        {/* Favorites Grid or Empty State */}
        {isLoading ? (
          <p className="text-muted-foreground">Loading favorites...</p>
        ) : isError ? (
          <p className="text-muted-foreground">Failed to load favorites.</p>
        ) : favoriteTeachers.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {favoriteTeachers.map((teacher) => (
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
            title="No favorite teachers yet"
            message="Start exploring and add your favorite teachers to this list."
            action={
              <Link to="/">
                <Button className="bg-accent hover:bg-accent/90">
                  Browse Teachers
                </Button>
              </Link>
            }
          />
        )}
      </div>
    </main>
  )
}
