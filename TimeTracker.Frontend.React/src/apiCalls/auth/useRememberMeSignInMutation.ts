import { useMutation } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost } from '../../helpers/fetch';
import { calculateTokenExpiry, getRememberMeExpiry } from '../../helpers/tokenExpiry';
import type { TokenResponse, UseRememberMeSignInMutationProps } from './types';

// Hook for remember me sign in mutation
export const useRememberMeSignInMutation = ({ setIsSignedIn }: UseRememberMeSignInMutationProps) => {
    return useMutation({
        mutationFn: async (rememberMeToken: string) => {
            return await executePost<TokenResponse>(`${import.meta.env.VITE_BASE_URL}/v1/auth/remember-me-signin`, { rememberMeToken });
        },
        onSuccess: (tokenResponse: TokenResponse) => {
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
};