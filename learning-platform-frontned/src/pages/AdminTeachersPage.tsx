import { Link } from 'react-router-dom'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { teacherService } from '../api/services/teacherService'
import { useTeachers } from '../hooks/useTeachers'
import { useLocation } from '../hooks/useLocation'

export default function AdminTeachersPage() {
  const queryClient = useQueryClient()
  const { location } = useLocation()
  const { data, isLoading, isError } = useTeachers({
    currency: location.currencyCode,
    page: 1,
    pageSize: 100,
  })

  const deleteMutation = useMutation({
    mutationFn: (teacherId: string) => teacherService.deleteTeacher(teacherId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['teachers'] })
    },
  })

  return (
    <main className="bg-background min-h-screen">
      <div className="max-w-5xl mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h2 className="text-3xl font-bold text-foreground">Admin · Teachers</h2>
            <p className="text-muted-foreground text-sm">Create, edit, and delete teachers.</p>
          </div>
          <Link
            to="/admin/teachers/new"
            className="px-4 py-2 rounded-md bg-accent text-white hover:bg-accent/90"
          >
            New teacher
          </Link>
        </div>

        {isLoading ? (
          <p className="text-muted-foreground">Loading…</p>
        ) : isError ? (
          <p className="text-muted-foreground">Failed to load teachers.</p>
        ) : (
          <div className="bg-card border border-border rounded-lg overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-muted">
                <tr>
                  <th className="text-left p-3">Name</th>
                  <th className="text-left p-3">Level</th>
                  <th className="text-left p-3">Base (USD)</th>
                  <th className="text-right p-3">Actions</th>
                </tr>
              </thead>
              <tbody>
                {(data?.items ?? []).map((t) => (
                  <tr key={t.id} className="border-t border-border">
                    <td className="p-3 font-medium text-foreground">{t.name}</td>
                    <td className="p-3 text-muted-foreground">{t.level}</td>
                    <td className="p-3 text-muted-foreground">{t.basePriceUsd}</td>
                    <td className="p-3">
                      <div className="flex justify-end gap-2">
                        <Link
                          to={`/admin/teachers/${t.id}/edit`}
                          className="px-3 py-1 rounded-md border border-border hover:bg-muted"
                        >
                          Edit
                        </Link>
                        <button
                          className="px-3 py-1 rounded-md border border-border hover:bg-muted disabled:opacity-50"
                          disabled={deleteMutation.isPending}
                          onClick={() => deleteMutation.mutate(t.id)}
                        >
                          Delete
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </main>
  )
}

