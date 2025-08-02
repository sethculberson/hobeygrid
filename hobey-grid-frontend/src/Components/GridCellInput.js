import React from 'react';
import { CheckCircle, XCircle } from 'lucide-react'; // Import icons

export default function GridCellInput({
  value, // Now an object {name: '', id: null, isCorrect: boolean?}
  row,
  col,
  onChange,
  onFocus,
  isActive,
  searchTerm,
  searchResults,
  isSearching,
  isGameActive,
  onPlayerSelect,
}) {
  // Determine cell background color based on validation result
  const cellBgClass = !isGameActive && value.isCorrect !== undefined
    ? (value.isCorrect ? 'bg-green-700' : 'bg-red-700')
    : 'bg-gray-700';

  return (
    <div className="relative">
      <input
        type="text"
        className={`w-full p-2 text-center text-white rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500 disabled:bg-gray-600 ${cellBgClass}`}
        value={value.name}
        onChange={(e) => onChange(e, row, col)}
        onFocus={() => onFocus(row, col)}
        disabled={!isGameActive}
        placeholder="Enter player"
      />
      {!isGameActive && value.isCorrect !== undefined && (
        <div className="absolute top-1/2 right-2 -translate-y-1/2">
          {value.isCorrect ? <CheckCircle className="text-green-300" size={20} /> : <XCircle className="text-red-300" size={20} />}
        </div>
      )}

      {isActive && searchResults.length > 0 && searchTerm.length > 2 && (
        <div className="absolute z-10 w-full bg-gray-700 border border-gray-600 rounded-md shadow-lg mt-1 max-h-48 overflow-y-auto">
          {isSearching ? (
            <div className="p-2 text-gray-400 text-center">Searching...</div>
          ) : (
            searchResults.map(player => (
              <div
                key={player.id}
                className="p-2 text-white hover:bg-purple-600 cursor-pointer"
                onClick={() => onPlayerSelect(player.name, player.id)}
              >
                {player.name}
              </div>
            ))
          )}
        </div>
      )}
    </div>
  );
}
