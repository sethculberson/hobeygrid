import { Grid3X3 } from 'lucide-react'

export default function NavBar() {
    return (
        <nav className='fixed left-0 top-0 w-full bg-red-700 flex justify-between items-center p-4'>
            {/* Wrap the title and icon in a div */}
            <div className="flex items-center">
                <h1 className="text-3xl font-extrabold text-white flex items-center">
                    <Grid3X3 className='mr-3' size={32}/>
                    Hobey Grid
                </h1>
            </div>
            
            {/* This element will be pushed to the right */}
            <h2 className="text-lg font-semibold text-white flex items-center">
                History
            </h2>
        </nav>
    );
}
