import {useRouteError, isRouteErrorResponse, useNavigate} from "react-router-dom";
import {AlertCircle, FileQuestion} from "lucide-react";
import {Button} from "@/shared/ui/button";
import {DvdBouncer} from "@/shared/ui/dvd-bouncer";

export function ErrorPage() {
    const error = useRouteError();
    const navigate = useNavigate();

    let title = "Oops! Something went wrong.";
    let message = "An unexpected error occurred. Our team has been notified.";
    let Icon = AlertCircle;

    if (isRouteErrorResponse(error) && error.status === 404) {
        title = "404 - Page Not Found";
        message = "The page you are looking for doesn't exist or has been moved.";
        Icon = FileQuestion;
    }

    return (
        <div
            className="relative flex min-h-screen flex-col items-center justify-center bg-background p-4 text-center overflow-hidden">

            <DvdBouncer icon={Icon}/>

            <div className="relative z-10 flex max-w-md flex-col items-center gap-6">

                <div className="rounded-full bg-muted p-6">
                    <Icon className="h-12 w-12 text-muted-foreground"/>
                </div>

                <div className="space-y-2">
                    <h1 className="text-3xl font-bold tracking-tight">{title}</h1>
                    <p className="text-muted-foreground">{message}</p>
                </div>

                <Button onClick={() => navigate(-1)} className="mt-4">
                    Go Back
                </Button>
            </div>
        </div>
    );
}