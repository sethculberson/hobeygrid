import React from 'react';
import { Grid3X3 } from 'lucide-react';
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
      <h1 className="text-5xl font-extrabold mb-8 text-orange-500 flex items-center">
        <Grid3X3 className="mr-4" size={48} />
        Hobey Grid
      </h1>

      <p className="text-lg mb-8 text-gray-300 text-center max-w-2xl">
        Test your college hockey knowledge! Fill the grid with players who fit both the row and column categories.
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