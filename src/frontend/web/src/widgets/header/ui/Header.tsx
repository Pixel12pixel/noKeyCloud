import { ThemeToggle } from "@/features/theme-switcher/ui/ThemeToggle"
import { Link } from "react-router-dom"

export function Header() {
    return (
        <header className="flex items-center justify-between p-4 border-b">
            <Link to="/" className="text-xl font-bold hover:opacity-80 transition-opacity">
                noKeyCloud
            </Link>

            <ThemeToggle />
        </header>
    )
}