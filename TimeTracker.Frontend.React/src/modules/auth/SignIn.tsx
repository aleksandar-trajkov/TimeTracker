import React, { useState } from 'react';
import Cookies from 'js-cookie';
import { useMutation } from '@tanstack/react-query';
import { Input, Checkbox } from '../../components/common';
// @ts-ignore
import { executePost } from '../../helpers/fetch'; 

interface SignInProps {
    setIsSignedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const SignIn: React.FC<SignInProps> = ({ setIsSignedIn }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    // React Query mutation for sign in
    const signInMutation = useMutation({
        mutationFn: async (credentials: { email: string; password: string; rememberMe: boolean }) => {
            return await executePost(`${import.meta.env.VITE_BASE_URL}/auth/signin`, credentials);
        },
        onSuccess: (tokenResponse) => {
            if (tokenResponse && tokenResponse.token) {
                Cookies.set('token', tokenResponse.token, { expires: 365 });
                if (rememberMe && tokenResponse.rememberMeToken) {
                    Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: 365 });
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
                        <button type='submit' className="btn btn-primary w-100" disabled={signInMutation.isPending}>
                            {signInMutation.isPending ? (
                                <>
                                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                    Signing In...
                                </>
                            ) : (
                                'Sign In'
                            )}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default SignIn;
