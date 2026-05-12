import { Heart } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import type { TeacherCardDto, TeacherLevel } from '../types'
import { TEACHER_LEVEL_LABEL } from '../types'
import { useFavoritesStore } from '../store'
import { Button } from './ui/button'

interface TeacherCardProps {
  teacher: TeacherCardDto
  onToggleFavorite: (teacherId: string, isFavorite: boolean) => void
}

export default function TeacherCard({ teacher, onToggleFavorite }: TeacherCardProps) {
  const navigate = useNavigate()
  const favorites = useFavoritesStore((s) => s.favorites)
  const isLiked = teacher.isFavorite || favorites.includes(teacher.id)

  const handleToggleFavorite = () => {
    onToggleFavorite(teacher.id, isLiked)
  }

  const handleBookNow = () => {
    navigate(`/teachers/${teacher.id}`)
  }

  const getLevelColor = (level: TeacherLevel) => {
    return level === 'Beginner'
      ? 'bg-green-100 text-green-800'
      : 'bg-purple-100 text-purple-800'
  }

  return (
    <div className="bg-card border border-border rounded-xl overflow-hidden hover:shadow-xl transition-all duration-300 hover:-translate-y-1 group">
      <div className="relative overflow-hidden bg-gradient-to-br from-accent/5 to-accent/10 p-6 flex items-center justify-between">
        <div className="flex-1">
          <h3 className="text-xl font-bold text-foreground group-hover:text-accent transition-colors">{teacher.name}</h3>
          <div className="flex items-center gap-2 mt-1">
            <div className="w-2 h-2 bg-green-500 rounded-full"></div>
            <span className="text-sm text-muted-foreground">Available now</span>
          </div>
        </div>
        <button
          onClick={handleToggleFavorite}
          className="bg-white/80 backdrop-blur-sm rounded-full p-3 shadow-md hover:shadow-lg hover:bg-white transition-all hover:scale-105"
          aria-label={isLiked ? 'Remove from favorites' : 'Add to favorites'}
        >
          <Heart
            className="w-5 h-5"
            fill={isLiked ? '#ef4444' : 'none'}
            color={isLiked ? '#ef4444' : '#6b7280'}
          />
        </button>
      </div>

      {/* Content */}
      <div className="p-6">
        {/* Level Badge */}
        <div className="mb-4">
          <span className={`inline-block px-4 py-2 rounded-full text-sm font-semibold ${getLevelColor(teacher.level)} shadow-sm`}>
            {TEACHER_LEVEL_LABEL[teacher.level]}
          </span>
        </div>

        {/* Price and Button */}
        <div className="flex items-center justify-between">
          <div className="text-xl font-bold text-accent">
            {teacher.currency} {Math.round(teacher.localizedPrice)}
            <span className="text-sm text-muted-foreground font-normal block">per hour</span>
          </div>
          <Button onClick={handleBookNow} className="bg-gradient-to-r from-accent to-accent/90 hover:from-accent/90 hover:to-accent shadow-md hover:shadow-lg transition-all">
            Book Now
          </Button>
        </div>
      </div>
    </div>
  )
}
