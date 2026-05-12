import { TeacherCardSkeleton } from './skeletons/TeacherCardSkeleton';

interface LoadingGridProps {
  count?: number;
}

export const LoadingGrid = ({ count = 6 }: LoadingGridProps) => {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {Array.from({ length: count }).map((_, i) => (
        <TeacherCardSkeleton key={i} />
      ))}
    </div>
  );
};
