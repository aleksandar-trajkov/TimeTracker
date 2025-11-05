import { Suspense, lazy } from 'react'
import { Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Layout from './Layout';
import Loading from './Loading';
import React from 'react';

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
      staleTime: 5 * 60 * 1000, // 5 minutes
      gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    },
  },
});

const App: React.FC = () => {
    const  [signedIn, setSignedIn] = React.useState<boolean>(false);

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
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}

export default App
