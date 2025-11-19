import { useMutation } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost } from '../../helpers/fetch';
import { calculateTokenExpiry, getRememberMeExpiry, getTokenUserDetails } from '../../helpers/token';
import type { TokenResponse, UseRememberMeSignInMutationProps } from './types';
import useUserStore from '../../stores/userStore';

// Hook for remember me sign in mutation
export const useRememberMeSignInMutation = ({ setIsSignedIn }: UseRememberMeSignInMutationProps) => {
    return useMutation({
        mutationFn: async (rememberMeToken: string) => {
            return await executePost<TokenResponse>(`${import.meta.env.VITE_BASE_URL}/v1/auth/remember-me-signin`, { rememberMeToken });
        },
        onSuccess: (tokenResponse: TokenResponse) => {
            if (tokenResponse && tokenResponse.token) {
                const tokenExpiryDays = calculateTokenExpiry();
                
                Cookies.set('token', tokenResponse.token, { expires: tokenExpiryDays });
                const decodedUser = getTokenUserDetails(tokenResponse.token);
                if (decodedUser) {
                    useUserStore.getState().setUser(decodedUser);
                }
                if (tokenResponse.rememberMeToken) {
                    Cookies.set('rememberMe', tokenResponse.rememberMeToken, { expires: getRememberMeExpiry() });
                }
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