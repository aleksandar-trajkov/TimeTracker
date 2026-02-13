import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import { Input, Checkbox, Button } from '../../components/common';
import isTokenValid from '../../helpers/token';
import { useSignInMutation, useRememberMeSignInMutation } from '../../apiCalls/auth'; 

interface SignInProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const SignIn: React.FC<SignInProps> = ({ setIsSignedIn }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    // React Query mutation for remember me sign in
    const rememberMeSignInMutation = useRememberMeSignInMutation({ setIsSignedIn });

    // Check for existing valid token on mount
    useEffect(() => {
        const checkExistingToken = async () => {
            try {
                const token = Cookies.get('token');
                if (token) {
                    const isValid = isTokenValid(token);
                    if (isValid) {
                        // Token exists and is valid, sign in immediately
                        setIsSignedIn(true);
                        return;
                    }
                }

                // Check remember me token if main token is invalid/missing
                const rememberMeToken = Cookies.get('rememberMe');
                if (rememberMeToken) {
                    const isRememberMeValid = isTokenValid(rememberMeToken);
                    if (isRememberMeValid) {
                        // Remember me token is valid, use mutation to sign in
                        rememberMeSignInMutation.mutate(rememberMeToken);
                        return;
                    } else {
                        // Remember me token is expired, remove it
                        Cookies.remove('rememberMe');
                    }
                }
            } catch (error) {
                console.error('Error checking existing tokens:', error);
                // Clear invalid tokens
                Cookies.remove('token');
                Cookies.remove('rememberMe');
            }
        };

        checkExistingToken();
    }, [setIsSignedIn, rememberMeSignInMutation]);

    // React Query mutation for sign in
    const signInMutation = useSignInMutation({ setIsSignedIn });

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        signInMutation.mutate({ email, password, rememberMe });
    };

    return (
        <div className="d-flex justify-content-center align-items-center min-vh-100 bg-dark">
            <div className="card shadow-lg bg-dark text-light border-secondary" style={{ width: '400px' }}>
                <div className="card-body p-4">
                    <h2 className="card-title text-center mb-4 text-light">Sign In</h2>
                    {signInMutation.isError && (
                        <div className="alert alert-danger" role="alert">
                            Sign in failed. Please check your credentials and try again.
                        </div>
                    )}
                    <form onSubmit={handleSubmit}>
                        <Input
                            id="email"
                            name="email"
                            type="text"
                            label="Email"
                            value={email}
                            onChange={setEmail}
                            placeholder="Enter your email"
                            required
                        />
                        
                        <Input
                            id="password"
                            name="password"
                            type="password"
                            label="Password"
                            value={password}
                            onChange={setPassword}
                            placeholder="Enter your password"
                            required
                        />
                        
                        <Checkbox
                            id="rememberMe"
                            name="rememberMe"
                            label="Remember me"
                            checked={rememberMe}
                            onChange={setRememberMe}
                        />
                        <Button
                            type="submit"
                            variant="primary"
                            fullWidth
                            disabled={signInMutation.isPending}
                            loading={signInMutation.isPending}
                            loadingText="Signing In..."
                        >
                            Sign In
                        </Button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default SignIn;
