import { useMutation } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost } from '../../helpers/fetch';
import { calculateTokenExpiry, getRememberMeExpiry } from '../../helpers/tokenExpiry';
import type { SignInCredentials, TokenResponse, UseSignInMutationProps } from './types';

// Hook for sign in mutation
export const useSignInMutation = ({ setIsSignedIn }: UseSignInMutationProps) => {
    return useMutation({
        mutationFn: async (credentials: SignInCredentials) => {
            return await executePost(`${import.meta.env.VITE_BASE_URL}/auth/signin`, credentials);
        },
        onSuccess: (tokenResponse: TokenResponse, variables: SignInCredentials) => {
            if (tokenResponse && tokenResponse.token) {
                // Use utility function to calculate token expiry
                const tokenExpiryDays = calculateTokenExpiry();
                
                Cookies.set('token', tokenResponse.token, { expires: tokenExpiryDays });
                if (variables.rememberMe && tokenResponse.rememberMeToken) {
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
};