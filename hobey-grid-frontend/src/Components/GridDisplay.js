import React from 'react';
import GridCellInput from './GridCellInput';

export default function GridDisplay({
  grid,
  rowCategories,
  colCategories,
  activeCell,
  searchTerm,
  searchResults,
  isSearching,
  isGameActive,
  handleCellChange,
  handlePlayerSelect,
}) {
  return (
    <div className="grid grid-cols-4 gap-2 p-4 bg-gray-400 rounded-lg shadow-xl">
      <div className="p-2 text-center font-bold text-gray-400"></div>
      {colCategories.map((cat, index) => (
        <div key={`col-cat-${index}`} className="p-2 text-center font-semibold text-red-100 bg-red-500 rounded-md">
          {cat}
        </div>
      ))}
      {/* Rows */}
      {rowCategories.map((rowCat, rowIndex) => (
        <React.Fragment key={`row-${rowIndex}`}>
          <div className="p-2 text-center font-semibold text-red-100 bg-red-500 rounded-md">
            {rowCat}
          </div>
          {grid[rowIndex].map((cellValue, colIndex) => (
            <GridCellInput
              key={`cell-${rowIndex}-${colIndex}`}
              value={cellValue}
              row={rowIndex}
              col={colIndex}
              onChange={handleCellChange}
              onFocus={() => {  }}
              isActive={activeCell && activeCell.row === rowIndex && activeCell.col === colIndex}
              searchTerm={searchTerm}
              searchResults={searchResults}
              isSearching={isSearching}
              isGameActive={isGameActive}
              onPlayerSelect={handlePlayerSelect}
            />
          ))}
        </React.Fragment>
      ))}
    </div>
  );
}
