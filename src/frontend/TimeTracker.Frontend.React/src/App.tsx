import { Suspense, lazy } from 'react'
import React from 'react'
import { Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Cookies from 'js-cookie';
import Layout from './Layout';
import Loading from './Loading';
import { ToastContainer } from './components/notifications';
import isTokenValid from './helpers/token';

// Lazy load modules for better performance
const SignIn = lazy(() => import('./modules/auth/SignIn'));
const TimeEntriesModule = lazy(() => import('./modules/timeEntries/TimeEntriesModule'));
const ReportsModule = lazy(() => import('./modules/reports/ReportsModule'));
const CategoriesModule = lazy(() => import('./modules/categories/CategoriesModule'));
const SettingsModule = lazy(() => import('./modules/settings/SettingsModule'));

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: parseInt(import.meta.env.VITE_QUERY_STALE_TIME),
      gcTime: parseInt(import.meta.env.VITE_QUERY_GC_TIME)
    },
  },
});

const getInitialSignedInState = (): boolean => {
  try {
    const token = Cookies.get('token');
    return !!token && isTokenValid(token);
  } catch {
    return false;
  }
};

const App: React.FC = () => {
    const [signedIn, setSignedIn] = React.useState<boolean>(getInitialSignedInState);

  return (
    <QueryClientProvider client={queryClient}>
      <Suspense fallback={<Loading />}>
        {!signedIn ? (
          <Suspense fallback={<Loading />}>
            <SignIn setIsSignedIn={setSignedIn} />
          </Suspense>
        ) : (
            <Layout>
                <Routes>
                  <Route path="/categories" element={ <CategoriesModule /> } />
                  <Route path="/settings" element={ <SettingsModule /> } />
                  <Route path="/reports" element={ <ReportsModule /> } />
                  <Route path="*" element={ <TimeEntriesModule /> } />
                </Routes>
            </Layout>
        )}
      </Suspense>
      <ToastContainer />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}

export default App
