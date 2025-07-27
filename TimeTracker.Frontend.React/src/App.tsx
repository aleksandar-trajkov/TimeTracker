import { Suspense, useState } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Layout from './Layout';
import Loading from './Loading';
import { SignIn } from './modules/auth';

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
        {!signedIn ? <SignIn setIsSignedIn={setSignedIn}></SignIn> : 
        <BrowserRouter>
          <Layout>
              <Routes>
                <Route path="/" element={<div>Home</div>} />
              </Routes>
            </Layout>
        </BrowserRouter>
    }
      </Suspense>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}

export default App
