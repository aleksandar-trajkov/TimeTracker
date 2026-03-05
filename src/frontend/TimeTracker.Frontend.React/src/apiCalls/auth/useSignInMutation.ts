import { useMutation } from '@tanstack/react-query';
import { executePost } from '../../helpers/fetch';
import { applyTokenResponse } from '../../helpers/token';
import type { SignInRequest, TokenResponse, UseSignInMutationProps } from './types';

// Hook for sign in mutation
export const useSignInMutation = ({ setIsSignedIn }: UseSignInMutationProps) => {
    return useMutation({
        mutationFn: async (credentials: SignInRequest) => {
            return await executePost<TokenResponse>(`${import.meta.env.VITE_BASE_URL}/v1/auth/signin`, credentials);
        },
        onSuccess: (tokenResponse: TokenResponse, variables: SignInRequest) => {
            if (tokenResponse && tokenResponse.token) {
                applyTokenResponse(
                    tokenResponse.token,
                    variables.rememberMe ? tokenResponse.rememberMeToken : null
                );
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