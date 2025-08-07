import React from 'react';
import { Grid3X3 } from 'lucide-react'
import useHobeyGridGame from './Hooks/useHobeyGridGame';
import GridDisplay from './Components/GridDisplay';
import GameControls from './Components/GameControls';

export default function App() {
  const {
    grid,
    rowCategories,
    colCategories,
    activeCell,
    searchTerm,
    searchResults,
    message,
    isGameActive,
    isSearching,
    handleCellChange,
    handlePlayerSelect,
    handleSubmit,
    resetGame,
    fetchDailyGrid, // Added to fetch daily grid on load
  } = useHobeyGridGame();

  // Fetch daily grid on initial load
  React.useEffect(() => {
    fetchDailyGrid();
  }, [fetchDailyGrid]);


  return (
    <div className="min-h-screen bg-gray-200 text-zinc-900 flex flex-col items-center justify-center p-4 font-inter">
      <nav className = 'fixed left-0 top-0 w-full bg-red-700'>
          <h1 className="text-3xl p-4 font-extrabold text-white flex items-center">
            <Grid3X3 className='mr-3' size={32}/>
            Hobey Grid
          </h1>
      </nav>

      <p className="text-lg mb-8 text-zinc-900 text-center max-w-3xl">
        Fill the grid with players who fit both the row and column categories!
      </p>

      <GridDisplay
        grid={grid}
        rowCategories={rowCategories}
        colCategories={colCategories}
        activeCell={activeCell}
        searchTerm={searchTerm}
        searchResults={searchResults}
        isSearching={isSearching}
        isGameActive={isGameActive}
        handleCellChange={handleCellChange}
        handlePlayerSelect={handlePlayerSelect}
      />

      <GameControls
        message={message}
        isGameActive={isGameActive}
        handleSubmit={handleSubmit}
        resetGame={resetGame}
      />

    </div>
  );
}