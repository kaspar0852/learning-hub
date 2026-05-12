import { Heart } from 'lucide-react';

interface EmptyStateProps {
  icon?: React.ReactNode;
  title?: string;
  message?: string;
  action?: React.ReactNode;
}

export const EmptyState = ({
  icon,
  title = 'No results found',
  message = 'Try adjusting your filters or search query',
  action,
}: EmptyStateProps) => {
  return (
    <div className="flex flex-col items-center justify-center py-12 px-4">
      <div className="mb-4">
        {icon || <Heart className="w-16 h-16 text-gray-300" />}
      </div>
      <h3 className="text-lg font-semibold text-gray-900 mb-2">{title}</h3>
      <p className="text-gray-600 text-center mb-6 max-w-md">{message}</p>
      {action}
    </div>
  );
};
