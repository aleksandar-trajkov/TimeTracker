import { useMutation, useQueryClient } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost, executePut, executeDelete } from '../../helpers/fetch';
import { showNotification } from '../../helpers/notifications';
import useUserStore from '../../stores/userStore';

export interface CategoryMutationData {
    id?: string;
    name: string;
    description?: string;
}

interface UseCategoryMutationProps {
    onSuccess?: () => void;
}

export const useCategoryMutation = ({ onSuccess }: UseCategoryMutationProps = {}) => {
    const queryClient = useQueryClient();
    const organizationId = useUserStore.getState().user?.organizationId;

    return useMutation({
        mutationFn: async (data: CategoryMutationData) => {
            const token = Cookies.get('token');
            const baseUrl = import.meta.env.VITE_BASE_URL;

            if (data.id) {
                await executePut(
                    `${baseUrl}/v1/categories/${data.id}`,
                    { name: data.name, description: data.description },
                    token
                );
                return data;
            } else {
                return await executePost(
                    `${baseUrl}/v1/categories`,
                    { name: data.name, description: data.description, organizationId },
                    token
                );
            }
        },
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({ queryKey: ['categories'] });
            const isUpdate = !!variables.id;
            showNotification.success(
                isUpdate ? 'Category updated' : 'Category created',
                isUpdate ? 'The category has been successfully updated.' : 'The category has been successfully created.'
            );
            if (onSuccess) onSuccess();
        },
        onError: (error: Error, variables) => {
            const isUpdate = !!variables.id;
            showNotification.apiError(
                isUpdate ? 'Failed to update category' : 'Failed to create category',
                error
            );
        },
        gcTime: 0,
        retry: false
    });
};

export const useDeleteCategoryMutation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (id: string) => {
            const token = Cookies.get('token');
            await executeDelete(`${import.meta.env.VITE_BASE_URL}/v1/categories/${id}`, token);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['categories'] });
            showNotification.success('Category deleted', 'The category has been successfully deleted.');
        },
        onError: (error: Error) => {
            showNotification.apiError('Failed to delete category', error);
        },
        gcTime: 0,
        retry: false
    });
};
