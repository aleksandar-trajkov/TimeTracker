import { useMutation } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost } from '../../helpers/fetch';
import { applyTokenResponse } from '../../helpers/token';
import type { TokenResponse, UseRememberMeSignInMutationProps } from './types';

// Hook for remember me sign in mutation
export const useRememberMeSignInMutation = ({ setIsSignedIn }: UseRememberMeSignInMutationProps) => {
    return useMutation({
        mutationFn: async (rememberMeToken: string) => {
            return await executePost<TokenResponse>(`${import.meta.env.VITE_BASE_URL}/v1/auth/remember-me-signin`, { rememberMeToken });
        },
        onSuccess: (tokenResponse: TokenResponse) => {
            if (tokenResponse && tokenResponse.token) {
                applyTokenResponse(tokenResponse.token, tokenResponse.rememberMeToken);
                setIsSignedIn(true);
            }
        },
        onError: (error) => {
            console.error('Remember me sign in failed:', error);
            Cookies.remove('rememberMe');
        },
        gcTime: 0,
        retry: false
    });
};