import { Code2, Globe } from "lucide-react";

export function Footer() {
    return (
        <footer className="w-full border-t bg-background py-2 md:py-2">
            <div className="container mx-auto flex flex-col items-center justify-between gap-4 px-4 md:flex-row">

                <p className="text-center text-sm leading-loose text-muted-foreground md:text-left">
                    Built securely by <span className="font-medium text-foreground">noKeyCloud</span> team.
                </p>

                <div className="flex items-center gap-4">
                    <a
                        href="https://github.com/Pixel12pixel/noKeyCloud"
                        target="_blank"
                        rel="noreferrer"
                        className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
                    >
                        <Code2 className="h-4 w-4" />
                        GitHub
                    </a>

                    <a
                        href="https://nokeycloud.com"
                        target="_blank"
                        rel="noreferrer"
                        className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
                    >
                        <Globe className="h-4 w-4" />
                        Project Page
                    </a>
                </div>

            </div>
        </footer>
    );
}