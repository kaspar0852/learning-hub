export const TeacherCardSkeleton = () => {
  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden animate-pulse">
      <div className="h-48 bg-gradient-to-r from-gray-200 to-gray-100" />
      
      <div className="p-4">
        <div className="h-5 bg-gray-200 rounded w-3/4 mb-2" />
        <div className="h-4 bg-gray-200 rounded w-1/2 mb-3" />
        
        <div className="space-y-2 mb-4">
          <div className="h-3 bg-gray-200 rounded w-full" />
          <div className="h-3 bg-gray-200 rounded w-5/6" />
        </div>

        <div className="flex items-center justify-between mb-3">
          <div className="h-4 bg-gray-200 rounded w-20" />
          <div className="h-4 bg-gray-200 rounded w-16" />
        </div>

        <div className="flex gap-2">
          <div className="flex-1 h-10 bg-gray-200 rounded" />
          <div className="w-10 h-10 bg-gray-200 rounded" />
        </div>
      </div>
    </div>
  );
};
