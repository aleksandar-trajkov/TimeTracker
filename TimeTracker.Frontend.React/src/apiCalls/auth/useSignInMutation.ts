import { useMutation } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost } from '../../helpers/fetch';
import { calculateTokenExpiry, getRememberMeExpiry } from '../../helpers/token';
import type { SignInRequest, TokenResponse, UseSignInMutationProps } from './types';

// Hook for sign in mutation
export const useSignInMutation = ({ setIsSignedIn }: UseSignInMutationProps) => {
    return useMutation({
        mutationFn: async (credentials: SignInRequest) => {
            return await executePost<TokenResponse>(`${import.meta.env.VITE_BASE_URL}/v1/auth/signin`, credentials);
        },
        onSuccess: (tokenResponse: TokenResponse, variables: SignInRequest) => {
            if (tokenResponse && tokenResponse.token) {
                const tokenExpiryDays = calculateTokenExpiry();
                
                Cookies.set('token', tokenResponse.token, { expires: tokenExpiryDays });
                if (variables.rememberMe && tokenResponse.rememberMeToken) {
                    Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: getRememberMeExpiry() });
                }
                setIsSignedIn(true);
            }
        },
        onError: (error) => {
            console.error('Sign in failed:', error);
        },
        gcTime: 0,
        retry: false
    });
};