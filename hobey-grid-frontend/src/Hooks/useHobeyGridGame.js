import { useState, useCallback } from 'react';

const API_BASE_URL = 'https://hobeygrid.azurewebsites.net';

export default function useHobeyGridGame() {
  // Grid state now stores objects {name: 'Player Name', id: 'player-uuid'}
  // Initialize with null or empty objects for cells
  const [grid, setGrid] = useState(Array(3).fill(null).map(() => Array(3).fill(null).map(() => ({ name: '', id: null }))));
  const [activeCell, setActiveCell] = useState(null); // {row: 0, col: 0}
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState([]); // Search results will now contain {id, fullName}
  const [message, setMessage] = useState('');
  const [isGameActive, setIsGameActive] = useState(true);
  const [isSearching, setIsSearching] = useState(false);
  const [gridInstanceId, setGridInstanceId] = useState(null); // To store the ID of the current daily grid

  // Categories will now be fetched from the backend
  const [rowCategories, setRowCategories] = useState(Array(3).fill(''));
  const [colCategories, setColCategories] = useState(Array(3).fill(''));

  // Fetch daily grid on component mount
  const fetchDailyGrid = useCallback(async () => {
    setMessage('Loading today\'s grid...');
    try {
      const response = await fetch(`${API_BASE_URL}/api/Grid/daily`);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const data = await response.json();
      setRowCategories([JSON.parse(data.rowCategory1).Name, JSON.parse(data.rowCategory2).Name, JSON.parse(data.rowCategory3).Name]);
      setColCategories([JSON.parse(data.colCategory1).Name, JSON.parse(data.colCategory2).Name, JSON.parse(data.colCategory3).Name]);
      setGridInstanceId(data.gridId);
      setMessage('Grid loaded!');
    } catch (error) {
      console.error("Error fetching daily grid:", error);
      setMessage("Error loading grid. Please try again later.");
      setIsGameActive(false); // Disable game if grid can't be loaded
    }
  }, []); // Empty dependency array means this function is stable and won't re-create

  const fetchSearchResults = useCallback(async (query) => {
    if (query.length < 3) {
      setSearchResults([]);
      setIsSearching(false);
      return;
    }
    setIsSearching(true);
    setMessage('');
    try {
      const response = await fetch(`${API_BASE_URL}/api/Players/search?name=${encodeURIComponent(query)}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      // Assuming backend returns an array of Player objects with PlayerId and FullName
      setSearchResults(data.map(player => ({ id: player.playerId, name: player.fullName })));

    } catch (error) {
      console.error("Error fetching search results:", error);
      setMessage("Error fetching players. Check console for details.");
      setSearchResults([]);
    } finally {
      setIsSearching(false);
    }
  }, []);

  const handleCellChange = (e, row, col) => {
    const value = e.target.value;
    const newGrid = grid.map((r, rIdx) =>
      r.map((cell, cIdx) => (rIdx === row && cIdx === col ? { name: value, id: null } : cell))
    );
    setGrid(newGrid);
    setSearchTerm(value);
    setActiveCell({ row, col });
    if (value.length > 2) {
      fetchSearchResults(value);
    } else {
      setSearchResults([]);
      setIsSearching(false);
    }
  };

  const handlePlayerSelect = (playerName, playerId) => {
    if (activeCell) {
      const newGrid = grid.map((r, rIdx) =>
        r.map((cell, cIdx) =>
          rIdx === activeCell.row && cIdx === activeCell.col ? { name: playerName, id: playerId } : cell
        )
      );
      setGrid(newGrid);
      setSearchTerm('');
      setSearchResults([]);
      setActiveCell(null);
    }
  };

  const handleSubmit = async () => {
    if (!gridInstanceId) {
      setMessage("Error: No grid loaded to submit.");
      return;
    }

    setMessage("Submitting grid for validation...");
    setIsGameActive(false); // Temporarily disable input

    const submittedPlayerGuesses = {};
    grid.forEach((row, rIdx) => {
      row.forEach((cell, cIdx) => {
        if (cell.id) { // Only send cells that have a player ID
          submittedPlayerGuesses[`${rIdx}_${cIdx}`] = cell.id;
        } else {
          submittedPlayerGuesses[`${rIdx}_${cIdx}`] = null; // Send null for empty/invalid cells
        }
      });
    });
    console.log("Submitted Player Guesses:", submittedPlayerGuesses);
    try {
      const response = await fetch(`${API_BASE_URL}/api/Grid/validate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          gridId: gridInstanceId,
          playerGuesses: submittedPlayerGuesses,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const validationResults = await response.json();
      console.log("Validation Results:", validationResults);

      // Update grid to show correct/incorrect status
      const updatedGrid = grid.map((row, rIdx) =>
        row.map((cell, cIdx) => {
          const cellKey = `${rIdx}_${cIdx}`;
          const isCorrect = validationResults[cellKey];
          return { ...cell, isCorrect: isCorrect }; // Add isCorrect flag to cell object
        })
      );
      setGrid(updatedGrid);

      setMessage("Grid submitted! Check cells for results.");

    } catch (error) {
      console.error("Error submitting grid:", error);
      setMessage("Error submitting grid. Check console for details.");
      setIsGameActive(true); // Re-enable if submission failed
    }
  };

  const resetGame = () => {
    setGrid(Array(3).fill(null).map(() => Array(3).fill(null).map(() => ({ name: '', id: null }))));
    setMessage('');
    setIsGameActive(true);
    setSearchTerm('');
    setSearchResults([]);
    setActiveCell(null);
    setGridInstanceId(null);
    setRowCategories(Array(3).fill(''));
    setColCategories(Array(3).fill(''));
    fetchDailyGrid(); // Fetch a new grid for the next game
  };

  return {
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
    fetchDailyGrid, // Expose fetchDailyGrid for App.js useEffect
  };
}