import { Moon, Sun } from "lucide-react"
import { Button } from "@/shared/ui/button"
import { useTheme } from "@/app/providers/ThemeProvider"

export function ThemeToggle() {
    const { theme, setTheme } = useTheme()

    const toggleTheme = () => {
        setTheme(theme === "light" ? "dark" : "light")
    }

    return (
        <Button
            variant="outline"
            size="icon"
            onClick={toggleTheme}
            className="rounded-full"
        >
            {theme === "dark" ? (
                <Sun className="h-[1.2rem] w-[1.2rem] text-orange-400" />
            ) : (
                <Moon className="h-[1.2rem] w-[1.2rem] text-slate-800" />
            )}
            <span className="sr-only">Toggle theme</span>
        </Button>
    )
}