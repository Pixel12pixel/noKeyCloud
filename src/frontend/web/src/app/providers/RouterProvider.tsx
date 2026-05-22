import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import { LoginPage } from '../../pages/login';

const router = createBrowserRouter([
    {
        path: '/',
        element: <Navigate to="/login" replace />,
},
{
    path: '/login',
        element: <LoginPage />,
},
]);

export function AppRouter() {
    return <RouterProvider router={router} />;
}