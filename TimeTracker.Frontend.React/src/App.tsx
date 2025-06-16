import { Suspense, useState } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './Layout';
import Loading from './Loading';
import SignIn from './SignIn';

const App: React.FC = () => {
    const  [signedIn, setSignedIn] = useState<boolean>(false);

  return (

    <Suspense fallback={<Loading />}>
      {!signedIn ? <SignIn setIsSignedIn={setSignedIn}></SignIn> : null}
      <BrowserRouter>
        <Layout>
            <Routes>
            </Routes>
          </Layout>
      </BrowserRouter>
    </Suspense>
  )
}

export default App
