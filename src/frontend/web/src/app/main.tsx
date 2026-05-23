import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { AppRouter } from './providers/RouterProvider.tsx';
import './styles/index.css'
import { ThemeProvider } from './providers/ThemeProvider';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ThemeProvider defaultTheme="dark" storageKey="app-ui-theme">
            <AppRouter />
        </ThemeProvider>
    </StrictMode>
);