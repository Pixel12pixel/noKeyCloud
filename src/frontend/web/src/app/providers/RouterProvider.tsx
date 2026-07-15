import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { LoginPage, loginAction } from "@/pages/login";
import { BaseLayout } from "../layouts/BaseLayout";
import { DashboardPage} from "@/pages/dashboard";
import {ErrorPage} from "@/pages/error";
import {RegisterPage, registerAction} from "@/pages/register";
import {AdminPanelPage} from "@/pages/admin-panel";

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
                path: "/register",
                element: <RegisterPage />,
                action: registerAction,
            },
            {
                path: "/folder/:folderId",
                element: <DashboardPage />,
            },
            {
                path: "/admin-panel",
                element: <AdminPanelPage />,
            },
        ],
    },
]);

export function AppRouter() {
    return <RouterProvider router={router} />;
}