import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { LoginPage, loginAction } from "../../pages/login/ui/LoginPage";
import { BaseLayout } from "../layouts/BaseLayout";
import { DashboardPage} from "@/pages/dashboard/ui/DashboardPage.tsx";
import {ErrorPage} from "@/pages/error/ui/ErrorPage.tsx";

const router = createBrowserRouter([
    {
        element: <BaseLayout />,
        errorElement: <ErrorPage />,
        children: [
            {
                path: "/",
                element: <DashboardPage />,
            },
            {
                path: "/login",
                element: <LoginPage />,
                action: loginAction,
            },
            {
                path: "/folder/:folderId",
                element: <DashboardPage />,
            },
        ],
    },
]);

export function AppRouter() {
    return <RouterProvider router={router} />;
}