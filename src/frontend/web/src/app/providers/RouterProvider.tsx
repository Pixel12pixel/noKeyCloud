import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import { LoginPage, loginAction } from "../../pages/login/ui/LoginPage";
import { BaseLayout } from "../layouts/BaseLayout";

const router = createBrowserRouter([
    {
        element: <BaseLayout />,
        children: [
            {
                path: "/",
                element: <Navigate to="/login" replace />,
            },
            {
                path: "/login",
                element: <LoginPage />,
                action: loginAction,
            },

        ],
    },
]);

export function AppRouter() {
    return <RouterProvider router={router} />;
}