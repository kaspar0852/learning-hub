import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import HomePage from './pages/HomePage'
import FavoritesPage from './pages/FavoritesPage'
import AdminTeachersPage from './pages/AdminTeachersPage'
import AdminTeacherFormPage from './pages/AdminTeacherFormPage'
import TeacherDetailPage from './pages/TeacherDetailPage'
import Header from './components/Header'
import { appConfig } from './config/app'

const queryClient = new QueryClient({
  defaultOptions: appConfig.queryClient.defaultOptions,
})

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Header />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/teachers/:id" element={<TeacherDetailPage />} />
          <Route path="/favorites" element={<FavoritesPage />} />
          <Route path="/admin/teachers" element={<AdminTeachersPage />} />
          <Route path="/admin/teachers/new" element={<AdminTeacherFormPage mode="create" />} />
          <Route path="/admin/teachers/:id/edit" element={<AdminTeacherFormPage mode="edit" />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
