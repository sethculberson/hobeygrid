import React from 'react';
import { CheckCircle, Trophy } from 'lucide-react';

export default function GameControls({ message, isGameActive, handleSubmit, resetGame }) {
  return (
    <>
      {message && (
        <div className={`mt-6 p-3 rounded-md text-center ${message.includes('Error') ? 'bg-red-600' : 'bg-green-600'} text-white`}>
          {message}
        </div>
      )}

      {isGameActive && (
        <button
          onClick={handleSubmit}
          className="mt-8 px-8 py-3 bg-red-500 hover:bg-red-600 text-white font-bold rounded-full shadow-lg transform transition-transform duration-200 hover:scale-105 focus:outline-none focus:ring-4 focus:ring-red-400"
        >
          Submit Grid <CheckCircle className="inline-block ml-2" size={20} />
        </button>
      )}

      {!isGameActive && (
        <button
          onClick={resetGame}
          className="mt-8 px-8 py-3 bg-red-500 hover:bg-red-600 text-white font-bold rounded-full shadow-lg transform transition-transform duration-200 hover:scale-105 focus:outline-none focus:ring-4 focus:ring-red-600"
        >
          Play Again <Trophy className="inline-block ml-2" size={20} />
        </button>
      )}
    </>
  );
}
