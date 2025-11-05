import { Suspense, useState, lazy } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Layout from './Layout';
import Loading from './Loading';

// Lazy load modules for better performance
const SignIn = lazy(() => import('./modules/auth/SignIn'));
const TimeEntriesModule = lazy(() => import('./modules/timeEntries/TimeEntriesModule'));
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
    const  [signedIn, setSignedIn] = useState<boolean>(false);

  return (
    <QueryClientProvider client={queryClient}>
      <Suspense fallback={<Loading />}>
        {!signedIn ? (
          <Suspense fallback={<Loading />}>
            <SignIn setIsSignedIn={setSignedIn} />
          </Suspense>
        ) : (
          <BrowserRouter>
            <Layout>
              <Suspense fallback={<Loading />}>
                <Routes>
                  <Route 
                    path="/categories" 
                    element={
                      <Suspense fallback={<Loading />}>
                        <CategoriesModule />
                      </Suspense>
                    } 
                  />
                  <Route 
                    path="/settings" 
                    element={
                      <Suspense fallback={<Loading />}>
                        <SettingsModule />
                      </Suspense>
                    } 
                  />
                  <Route 
                    path="*" 
                    element={
                      <Suspense fallback={<Loading />}>
                        <TimeEntriesModule />
                      </Suspense>
                    } 
                  />
                </Routes>
              </Suspense>
            </Layout>
          </BrowserRouter>
        )}
      </Suspense>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}

export default App
