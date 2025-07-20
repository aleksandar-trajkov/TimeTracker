import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import { useMutation } from '@tanstack/react-query';
import { Input, Checkbox, Button } from '../../components/common';
// @ts-ignore
import { executePost } from '../../helpers/fetch';
import isTokenValid from '../../helpers/tokenCheck';
import { calculateTokenExpiry, getRememberMeExpiry } from '../../helpers/tokenExpiry'; 

interface SignInProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const SignIn: React.FC<SignInProps> = ({ setIsSignedIn }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    // React Query mutation for remember me sign in
    const rememberMeSignInMutation = useMutation({
        mutationFn: async (rememberMeToken: string) => {
            return await executePost(`${import.meta.env.VITE_BASE_URL}/auth/remember-me-signin`, { rememberMeToken });
        },
        onSuccess: (tokenResponse) => {
            if (tokenResponse && tokenResponse.token) {
                // Use utility function to calculate token expiry
                const tokenExpiryDays = calculateTokenExpiry();
                
                Cookies.set('token', tokenResponse.token, { expires: tokenExpiryDays });
                if (tokenResponse.rememberMeToken) {
                    Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: getRememberMeExpiry() });
                }
                setIsSignedIn(true);
            }
        },
        onError: (error) => {
            console.error('Remember me sign in failed:', error);
            // Clear invalid remember me token
            Cookies.remove('rememberMe');
        },
        // Disable caching for remember me requests
        gcTime: 0,
        retry: false
    });

    // Check for existing valid token on mount
    useEffect(() => {
        const checkExistingToken = async () => {
            try {
                const token = Cookies.get('token');
                if (token) {
                    // isTokenValid returns true if token is EXPIRED, so we want the opposite
                    const isExpired = isTokenValid(token);
                    if (!isExpired) {
                        // Token exists and is valid, sign in immediately
                        setIsSignedIn(true);
                        return;
                    }
                }

                // Check remember me token if main token is invalid/missing
                const rememberMeToken = Cookies.get('rememberMe');
                if (rememberMeToken) {
                    const isRememberMeExpired = isTokenValid(rememberMeToken);
                    if (!isRememberMeExpired) {
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
    const signInMutation = useMutation({
        mutationFn: async (credentials: { email: string; password: string; rememberMe: boolean }) => {
            return await executePost(`${import.meta.env.VITE_BASE_URL}/auth/signin`, credentials);
        },
        onSuccess: (tokenResponse) => {
            if (tokenResponse && tokenResponse.token) {
                // Use utility function to calculate token expiry
                const tokenExpiryDays = calculateTokenExpiry();
                
                Cookies.set('token', tokenResponse.token, { expires: tokenExpiryDays });
                if (rememberMe && tokenResponse.rememberMeToken) {
                    // Remember me tokens can have longer expiry
                    Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: getRememberMeExpiry() });
                }
                setIsSignedIn(true);
            }
        },
        onError: (error) => {
            console.error('Sign in failed:', error);
        },
        // Disable caching for sign in requests
        gcTime: 0,
        retry: false
    });

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
                            type="email"
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
