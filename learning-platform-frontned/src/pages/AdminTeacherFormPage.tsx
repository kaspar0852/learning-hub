import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { teacherService } from '../api/services/teacherService'
import { useTeacherById } from '../hooks/useTeachers'
import type { CreateTeacherDto, TeacherLevel, UpdateTeacherDto } from '../types'
import { TEACHER_LEVEL_LABEL } from '../types'

type Mode = 'create' | 'edit'

export default function AdminTeacherFormPage({ mode }: { mode: Mode }) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const { id } = useParams()

  const isEdit = mode === 'edit'
  const teacherId = isEdit ? id ?? '' : ''

  const { data: existing, isLoading } = useTeacherById(teacherId)

  const initial = useMemo(() => {
    if (!isEdit || !existing) {
      return { name: '', level: 'Beginner' as TeacherLevel, basePriceUsd: 20 }
    }
    return {
      name: existing.name,
      level: existing.level,
      basePriceUsd: existing.basePriceUsd,
    }
  }, [isEdit, existing])

  const [name, setName] = useState(initial.name)
  const [level, setLevel] = useState<TeacherLevel>(initial.level)
  const [basePriceUsd, setBasePriceUsd] = useState<number>(initial.basePriceUsd)

  useEffect(() => {
    if (!isEdit || !existing) return
    setName(existing.name)
    setLevel(existing.level)
    setBasePriceUsd(existing.basePriceUsd)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isEdit, existing?.id])

  const createMutation = useMutation({
    mutationFn: (dto: CreateTeacherDto) => teacherService.createTeacher(dto),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['teachers'] })
      navigate('/admin/teachers')
    },
  })

  const updateMutation = useMutation({
    mutationFn: (dto: UpdateTeacherDto) => teacherService.updateTeacher(teacherId, dto),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['teachers'] })
      navigate('/admin/teachers')
    },
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const dto = { name, level, basePriceUsd: Number(basePriceUsd) }
    if (isEdit) updateMutation.mutate(dto)
    else createMutation.mutate(dto)
  }

  return (
    <main className="bg-background min-h-screen">
      <div className="max-w-2xl mx-auto px-4 py-8">
        <div className="mb-6">
          <Link to="/admin/teachers" className="text-accent hover:underline text-sm">
            Back
          </Link>
          <h2 className="text-3xl font-bold text-foreground mt-2">
            {isEdit ? 'Edit teacher' : 'Create teacher'}
          </h2>
        </div>

        {isEdit && isLoading ? (
          <p className="text-muted-foreground">Loading…</p>
        ) : (
          <form onSubmit={handleSubmit} className="bg-card border border-border rounded-lg p-6 space-y-4">
            <div>
              <label className="block text-sm font-medium text-foreground mb-2">Name</label>
              <input
                className="w-full px-3 py-2 border border-input rounded-md bg-background text-foreground"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-2">Level</label>
              <select
                className="w-full px-3 py-2 border border-input rounded-md bg-background text-foreground"
                value={level}
                onChange={(e) => setLevel(e.target.value as TeacherLevel)}
              >
                <option value="Beginner">{TEACHER_LEVEL_LABEL['Beginner']}</option>
                <option value="Advanced">{TEACHER_LEVEL_LABEL['Advanced']}</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-2">Base price (USD)</label>
              <input
                type="number"
                step="0.01"
                className="w-full px-3 py-2 border border-input rounded-md bg-background text-foreground"
                value={basePriceUsd}
                onChange={(e) => setBasePriceUsd(Number(e.target.value))}
              />
            </div>

            <div className="pt-2 flex gap-3">
              <button
                type="submit"
                className="px-4 py-2 rounded-md bg-accent text-white hover:bg-accent/90 disabled:opacity-50"
                disabled={createMutation.isPending || updateMutation.isPending}
              >
                Save
              </button>
              {(createMutation.isError || updateMutation.isError) && (
                <p className="text-sm text-destructive">Save failed. Check required fields.</p>
              )}
            </div>
          </form>
        )}
      </div>
    </main>
  )
}

