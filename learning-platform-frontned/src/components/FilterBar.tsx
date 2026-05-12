import { Search } from 'lucide-react'
import { Input } from './ui/input'

interface FilterBarProps {
  onSearchChange: (value: string) => void
  onLevelChange: (value: string) => void
  searchValue: string
  levelValue: string
}

export default function FilterBar({
  onSearchChange,
  onLevelChange,
  searchValue,
  levelValue,
}: FilterBarProps) {
  return (
    <div className="bg-card border border-border rounded-lg p-6 mb-8">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Search */}
        <div>
          <label className="block text-sm font-medium text-foreground mb-2">
            Search by name
          </label>
          <div className="relative">
            <Search className="absolute left-3 top-3 w-4 h-4 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Find a teacher..."
              value={searchValue}
              onChange={(e) => onSearchChange(e.target.value)}
              className="pl-10"
            />
          </div>
        </div>

        {/* Level */}
        <div>
          <label className="block text-sm font-medium text-foreground mb-2">
            Level
          </label>
          <select
            value={levelValue}
            onChange={(e) => onLevelChange(e.target.value)}
            className="w-full px-3 py-2 border border-input rounded-md bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-accent"
          >
            <option value="">All Levels</option>
            <option value="Beginner">Beginner</option>
            <option value="Advanced">Advanced</option>
          </select>
        </div>
      </div>
    </div>
  )
}
