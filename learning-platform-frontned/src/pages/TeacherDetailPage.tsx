import { useParams, useNavigate, Link } from 'react-router-dom'
import { ArrowLeft, Heart, Star, Clock, Globe } from 'lucide-react'
import { Button } from '../components/ui/button'
import { useFavoritesStore } from '../store'
import { useFavorites } from '../hooks/useFavorites'
import { useTeachers } from '../hooks/useTeachers'
import { useLocation } from '../hooks/useLocation'
import { TEACHER_LEVEL_LABEL } from '../types'
import { useState } from 'react'

export default function TeacherDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { addFavorite, removeFavorite } = useFavorites()
  const favorites = useFavoritesStore((s) => s.favorites)
  const { location } = useLocation()
  const { data, isLoading, isError } = useTeachers({ 
    currency: location.currencyCode,
    pageSize: 100 
  })
  
  const [selectedSlot, setSelectedSlot] = useState<string | null>(null)

  const teacher = data?.items.find(t => t.id === id)
  const isFavorite = teacher?.isFavorite || favorites.includes(id || '')

  if (isLoading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-accent"></div>
      </div>
    )
  }

  if (isError || !teacher) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-foreground mb-4">Teacher not found</h2>
          <Link to="/" className="text-accent hover:underline">
            Back to teachers
          </Link>
        </div>
      </div>
    )
  }

  const handleToggleFavorite = () => {
    if (id) {
      isFavorite ? removeFavorite(id) : addFavorite(id)
    }
  }

  const handleBookNow = () => {
    // In a real app, this would open a booking modal or navigate to booking page
    alert('Booking functionality would be implemented here!')
  }

  const timeSlots = [
    '9:00 AM', '10:00 AM', '11:00 AM', '2:00 PM', '3:00 PM', '4:00 PM', '5:00 PM', '6:00 PM'
  ]

  return (
    <div className="min-h-screen bg-background">
      <div className="max-w-4xl mx-auto px-4 py-8">
        {/* Back Button */}
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-muted-foreground hover:text-foreground mb-6 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Back to teachers
        </button>

        {/* Teacher Card */}
        <div className="bg-card border border-border rounded-xl overflow-hidden shadow-sm">
          {/* Header */}
          <div className="bg-gradient-to-r from-accent/10 to-accent/5 p-8">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <div className="flex items-center gap-4 mb-4">
                  <h1 className="text-3xl font-bold text-foreground">{teacher.name}</h1>
                  <button
                    onClick={handleToggleFavorite}
                    className="bg-white rounded-full p-3 shadow-md hover:shadow-lg transition-all"
                    aria-label={isFavorite ? 'Remove from favorites' : 'Add to favorites'}
                  >
                    <Heart
                      className="w-5 h-5"
                      fill={isFavorite ? '#ef4444' : 'none'}
                      color={isFavorite ? '#ef4444' : '#6b7280'}
                    />
                  </button>
                </div>
                
                <div className="flex items-center gap-6 text-sm text-muted-foreground">
                  <div className="flex items-center gap-1">
                    <Star className="w-4 h-4 text-yellow-500 fill-yellow-500" />
                    <span>4.8 (124 reviews)</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <Globe className="w-4 h-4" />
                    <span>Online teacher</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <Clock className="w-4 h-4" />
                    <span>500+ lessons taught</span>
                  </div>
                </div>
              </div>
              
              <div className="text-right">
                <div className="text-3xl font-bold text-accent">
                  {teacher.currency} {teacher.localizedPrice}
                </div>
                <div className="text-sm text-muted-foreground">per hour</div>
              </div>
            </div>
          </div>

          {/* Content */}
          <div className="p-8">
            <div className="grid md:grid-cols-3 gap-8">
              {/* Main Content */}
              <div className="md:col-span-2 space-y-6">
                {/* Level Badge */}
                <div>
                  <span className="inline-block px-4 py-2 rounded-full text-sm font-semibold bg-gradient-to-r from-blue-100 to-purple-100 text-blue-800">
                    {TEACHER_LEVEL_LABEL[teacher.level]}
                  </span>
                </div>

                {/* About */}
                <div>
                  <h3 className="text-xl font-semibold text-foreground mb-3">About</h3>
                  <p className="text-muted-foreground leading-relaxed">
                    {teacher.name} is an experienced English teacher with a passion for helping students achieve their language learning goals. 
                    With a focus on conversational skills and practical applications, lessons are tailored to each student's individual needs and learning style.
                    Whether you're preparing for exams, improving your business English, or simply want to become more confident in your English abilities, 
                    {teacher.name} provides a supportive and engaging learning environment.
                  </p>
                </div>

                {/* Specialties */}
                <div>
                  <h3 className="text-xl font-semibold text-foreground mb-3">Specialties</h3>
                  <div className="flex flex-wrap gap-2">
                    {['Business English', 'Conversation Practice', 'Exam Preparation', 'Pronunciation', 'Grammar'].map((specialty) => (
                      <span
                        key={specialty}
                        className="px-3 py-1 bg-muted text-muted-foreground rounded-md text-sm"
                      >
                        {specialty}
                      </span>
                    ))}
                  </div>
                </div>

                {/* Education */}
                <div>
                  <h3 className="text-xl font-semibold text-foreground mb-3">Education & Certifications</h3>
                  <div className="space-y-2">
                    <div className="flex items-start gap-3">
                      <div className="w-2 h-2 bg-accent rounded-full mt-2"></div>
                      <div>
                        <div className="font-medium text-foreground">TEFL Certificate</div>
                        <div className="text-sm text-muted-foreground">Teaching English as a Foreign Language</div>
                      </div>
                    </div>
                    <div className="flex items-start gap-3">
                      <div className="w-2 h-2 bg-accent rounded-full mt-2"></div>
                      <div>
                        <div className="font-medium text-foreground">Bachelor's Degree</div>
                        <div className="text-sm text-muted-foreground">English Literature</div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Booking Sidebar */}
              <div className="space-y-6">
                <div className="bg-muted/50 rounded-lg p-6">
                  <h3 className="text-lg font-semibold text-foreground mb-4">Book a Lesson</h3>
                  
                  {/* Time Slots */}
                  <div className="mb-6">
                    <h4 className="text-sm font-medium text-foreground mb-3">Available Time Slots</h4>
                    <div className="grid grid-cols-2 gap-2">
                      {timeSlots.map((slot) => (
                        <button
                          key={slot}
                          onClick={() => setSelectedSlot(slot)}
                          className={`px-3 py-2 text-sm rounded-md border transition-colors ${
                            selectedSlot === slot
                              ? 'bg-accent text-white border-accent'
                              : 'bg-background text-foreground border-border hover:bg-accent/10'
                          }`}
                        >
                          {slot}
                        </button>
                      ))}
                    </div>
                  </div>

                  {/* Book Button */}
                  <Button 
                    onClick={handleBookNow}
                    disabled={!selectedSlot}
                    className="w-full bg-accent hover:bg-accent/90"
                  >
                    {selectedSlot ? `Book for ${selectedSlot}` : 'Select a time slot'}
                  </Button>

                  <div className="mt-4 text-xs text-muted-foreground text-center">
                    Free cancellation up to 24 hours before
                  </div>
                </div>

                {/* Quick Stats */}
                <div className="bg-muted/50 rounded-lg p-6">
                  <h3 className="text-lg font-semibold text-foreground mb-4">Quick Stats</h3>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-sm text-muted-foreground">Response Time</span>
                      <span className="text-sm font-medium text-foreground">&lt; 1 hour</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-sm text-muted-foreground">Languages</span>
                      <span className="text-sm font-medium text-foreground">English, Spanish</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-sm text-muted-foreground">Students</span>
                      <span className="text-sm font-medium text-foreground">200+</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
