import { Link } from 'react-router-dom'
import { Heart } from 'lucide-react'
import { useFavoritesStore } from '../store'

export default function Header() {
  const favorites = useFavoritesStore((state) => state.favorites)

  return (
    <header className="bg-card/80 backdrop-blur-sm border-b border-border sticky top-0 z-40 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 py-4 flex items-center justify-between">
        <Link to="/" className="flex items-center gap-2 group">
          <div className="w-8 h-8 bg-gradient-to-br from-accent to-accent/80 rounded-lg flex items-center justify-center text-white font-bold shadow-md group-hover:shadow-lg transition-all">
            T
          </div>
          <h1 className="text-2xl font-bold text-foreground group-hover:text-accent transition-colors">TeachHub</h1>
        </Link>
        
        <nav className="flex items-center gap-6">
          <Link to="/" className="text-foreground hover:text-accent transition-colors font-medium">
            Teachers
          </Link>
          <Link to="/admin/teachers" className="text-foreground hover:text-accent transition-colors font-medium">
            Admin
          </Link>
          <Link 
            to="/favorites" 
            className="flex items-center gap-2 text-foreground hover:text-accent transition-colors font-medium group"
          >
            <Heart className="w-5 h-5 group-hover:scale-110 transition-transform" fill={favorites.length > 0 ? '#ef4444' : 'none'} color={favorites.length > 0 ? '#ef4444' : 'currentColor'} />
            <span>Favorites</span>
            {favorites.length > 0 && (
              <span className="bg-gradient-to-r from-accent to-accent/80 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center shadow-sm">
                {favorites.length}
              </span>
            )}
          </Link>
        </nav>
      </div>
    </header>
  )
}
