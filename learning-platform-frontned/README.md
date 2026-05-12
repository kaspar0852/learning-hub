# English Teacher Marketplace

A modern, production-grade React SPA for discovering and booking English language teachers worldwide. Built with React 19, Vite, and React Query.

## Features

✨ **Core Features**
- Browse and discover English teachers with detailed profiles
- Advanced filtering by expertise level, price range, and search query
- Save favorite teachers with persistent storage
- Real-time currency conversion based on user location
- Optimistic UI updates for instant feedback
- Responsive design for desktop, tablet, and mobile

🔧 **Technical Features**
- React Query for powerful data fetching and caching
- Zustand for lightweight state management
- Skeleton loading states for better UX
- Comprehensive error handling with retry logic
- Feature-based folder architecture
- Environment variable configuration
- TypeScript for type safety

## Getting Started

### Prerequisites
- Node.js 18+ 
- pnpm (recommended) or npm/yarn

### Installation

1. Clone the repository
```bash
git clone <repository-url>
cd english-teacher-marketplace
```

2. Install dependencies
```bash
pnpm install
```

3. Set up environment variables
```bash
cp .env.example .env.local
```

4. Start the development server
```bash
pnpm dev
```

The app will be available at `http://localhost:5173`

## Project Structure

```
src/
├── api/                    # API integration layer
│   ├── client.ts          # Axios instance and config
│   └── services/          # API service modules
│       ├── teacherService.ts
│       └── locationService.ts
├── components/            # Reusable React components
│   ├── ui/               # Base UI components
│   ├── skeletons/        # Loading skeleton components
│   ├── states/           # Empty and error state components
│   ├── Header.tsx
│   ├── TeacherCard.tsx
│   └── FilterBar.tsx
├── hooks/                # Custom React hooks
│   ├── useTeachers.ts
│   ├── useFavorites.ts
│   ├── useLocation.ts
│   └── useDebounce.ts
├── pages/                # Page components
│   ├── HomePage.tsx
│   └── FavoritesPage.tsx
├── config/               # App configuration
│   └── app.ts
├── data/                 # Mock and static data
│   └── mockData.ts
├── types/                # TypeScript type definitions
├── utils/                # Utility functions
│   └── currency.ts
├── store.ts              # Zustand state management
├── App.tsx               # Root component
├── main.tsx              # Entry point
└── index.css             # Global styles
```

## Available Scripts

```bash
# Start development server with HMR
pnpm dev

# Build for production
pnpm build

# Preview production build
pnpm preview
```

## Environment Variables

Create a `.env.local` file based on `.env.example`:

```env
# API Configuration
VITE_API_BASE_URL=https://api.example.com

# Feature Flags
VITE_ENABLE_LOCATION_DETECTION=true
VITE_ENABLE_PRICE_CONVERSION=true
VITE_DEFAULT_CURRENCY=USD
VITE_FEATURE_FAVORITES=true
VITE_FEATURE_SEARCH=true
```

## API Integration

Currently, the app uses mock data. To integrate with a real API:

1. Update API endpoints in `src/api/services/`
2. Replace mock data fetching with actual API calls
3. Add authentication headers if needed in `src/api/client.ts`

Example:
```typescript
// In src/api/services/teacherService.ts
export const teacherService = {
  getTeachers: async (): Promise<Teacher[]> => {
    const response = await apiClient.get('/teachers');
    return response.data;
  },
  // ... other methods
};
```

## Hooks Documentation

### useTeachers
Fetches all teachers with caching and retry logic.

```typescript
const { data: teachers, isLoading, isError, refetch } = useTeachers();
```

### useFavorites
Manages favorite teachers with optimistic updates.

```typescript
const {
  favorites,              // Array of favorite teacher IDs
  favoriteTeachers,       // Full teacher objects
  addFavorite,           // Add to favorites
  removeFavorite,        // Remove from favorites
  isFavorite,            // Check if teacher is favorited
} = useFavorites();
```

### useLocation
Detects user location and provides currency conversion.

```typescript
const {
  location,              // User location data
  convertPrice,          // Price conversion function
  isLoading,
  isError,
} = useLocation();

// Usage
const priceInLocalCurrency = convertPrice(priceUSD);
```

### useDebounce
Debounces input values for search optimization.

```typescript
const debouncedSearchQuery = useDebounce(searchQuery, 500);
```

## State Management

### Zustand Store (favorites-storage)
Persistent favorites management using localStorage:

```typescript
import { useFavoritesStore } from '@/store';

const { favorites, addFavorite, removeFavorite } = useFavoritesStore();
```

### React Query
Handles server state, caching, and data fetching:
- Automatic retry on failure (up to 3 times)
- Stale-while-revalidate behavior
- Optimistic updates for favorites
- Background refetching

## Loading States

The app provides professional loading experiences:

- **Skeleton Loaders**: Display while fetching teacher data
- **Error States**: User-friendly error messages with retry buttons
- **Empty States**: Contextual messages when no data matches filters

## Error Handling

- HTTP errors are caught and logged
- User-friendly error messages with retry options
- Automatic retry with exponential backoff
- Fallback to mock data on network failure

## Browser Support

- Chrome/Edge 88+
- Firefox 87+
- Safari 14+
- Mobile browsers (iOS Safari 14+, Chrome Android)

## Performance Optimizations

- Code splitting via Vite
- Lazy loading of pages with React Router
- Skeleton screens for perceived performance
- Optimistic UI updates
- React Query caching strategy
- Debounced search input

## Future Enhancements

- [ ] Teacher profile detail pages
- [ ] User authentication and profiles
- [ ] Booking and payment integration
- [ ] Teacher reviews and ratings system
- [ ] Real-time chat/messaging
- [ ] Video call integration
- [ ] Advanced analytics

## Contributing

1. Create a feature branch (`git checkout -b feature/amazing-feature`)
2. Commit your changes (`git commit -m 'Add amazing feature'`)
3. Push to the branch (`git push origin feature/amazing-feature`)
4. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions, please open an issue on GitHub or contact support.
