import { useMutation, useQueryClient } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executePost, executePut } from '../../helpers/fetch';
import { showNotification } from '../../stores/notificationStore';
import type { TimeEntry } from '../../types/TimeEntry';

export interface TimeEntryMutationData {
    id?: string;
    description: string;
    from: Date;
    to: Date;
    categoryId: string;
}

interface UseTimeEntryMutationProps {
    onSuccess?: (data: TimeEntry) => void;
    onError?: (error: Error) => void;
}

// Hook for creating or updating time entries
export const useTimeEntryMutation = ({ onSuccess, onError }: UseTimeEntryMutationProps = {}) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (timeEntryData: TimeEntryMutationData) => {
            const token = Cookies.get('token');
            const baseUrl = import.meta.env.VITE_BASE_URL;
            
            // Prepare the request body
            const requestBody = {
                description: timeEntryData.description,
                from: timeEntryData.from.toISOString(),
                to: timeEntryData.to.toISOString(),
                categoryId: timeEntryData.categoryId
            };

            if (timeEntryData.id) {
                // Update existing time entry
                return await executePut<TimeEntry>(
                    `${baseUrl}/v1/time-entries/${timeEntryData.id}`,
                    requestBody,
                    token
                );
            } else {
                // Create new time entry
                return await executePost<TimeEntry>(
                    `${baseUrl}/v1/time-entries`,
                    requestBody,
                    token
                );
            }
        },
        onSuccess: (data: TimeEntry, variables: TimeEntryMutationData) => {
            queryClient.invalidateQueries({ queryKey: ['timeEntries'] });
            
            const isUpdate = !!variables.id;
            showNotification.success(
                isUpdate ? 'Time entry updated' : 'Time entry created',
                isUpdate ? 'Your time entry has been successfully updated.' : 'Your time entry has been successfully created.'
            );
            
            if (onSuccess) {
                onSuccess(data);
            }
        },
        onError: (error: Error, variables: TimeEntryMutationData) => {
            const isUpdate = !!variables.id;
            showNotification.error(
                isUpdate ? 'Failed to update time entry' : 'Failed to create time entry',
                'Please try again. If the problem persists, contact support.'
            );
            
            if (onError) {
                onError(error);
            }
        },
        gcTime: 0,
        retry: false
    });
};