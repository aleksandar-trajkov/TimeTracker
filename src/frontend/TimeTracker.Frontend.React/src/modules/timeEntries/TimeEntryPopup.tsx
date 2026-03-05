import React, { useState } from 'react';
import type { TimeEntry } from '../../types/TimeEntry';
import { useCategoriesQuery } from '../../apiCalls/categories';
import { useTimeEntryMutation } from '../../apiCalls/timeEntries';
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../components/modal';
import { Input, Button, Dropdown } from '../../components/common';
import { DateTimePicker } from '../../components/date';
import useUserStore from '../../stores/userStore';

interface TimeEntryPopupProps {
    onClose: () => void;
    timeEntry?: TimeEntry | null;
    isOpen: boolean;
}

interface FormData {
    description: string;
    from: Date | null;
    to: Date | null;
    categoryId: string;
}

const TimeEntryPopup: React.FC<TimeEntryPopupProps> = ({ onClose, timeEntry = null, isOpen }) => {
    const organizationId = useUserStore(state => state.user?.organizationId);
    const { data: categories = [], isLoading: categoriesLoading, error: categoriesError } = useCategoriesQuery(organizationId || '');
    
    const timeEntryMutation = useTimeEntryMutation({
        onSuccess: () => {
            onClose();
        }
    });
    
    const [formData, setFormData] = useState<FormData>(() => ({
        description: timeEntry?.description || '',
        from: timeEntry?.from || null,
        to: timeEntry?.to || null,
        categoryId: timeEntry?.category?.id || '',
    }));

    const [validationError, setValidationError] = useState<string | null>(null);

    const handleSubmit = (e?: React.FormEvent) => {
        if (e) {
            e.preventDefault();
        }

        // Validation
        if (!formData.from || !formData.to || !formData.categoryId) {
            setValidationError('Start time, end time and category are required.');
            return;
        }

        setValidationError(null);

        // Prepare mutation data
        const mutationData = {
            id: timeEntry?.id,
            description: formData.description,
            from: formData.from,
            to: formData.to,
            categoryId: formData.categoryId,
        };

        timeEntryMutation.mutate(mutationData);
    };

    return (
        <Modal 
            isOpen={isOpen} 
            onClose={onClose} 
            size="md"
        >
            <ModalHeader
                title={timeEntry ? 'Edit Time Entry' : 'Add Time Entry'}
                onClose={onClose}
            />

            <ModalBody>
                <form onSubmit={handleSubmit}>
                    {validationError && (
                        <div className="alert alert-warning py-2" role="alert">
                            {validationError}
                        </div>
                    )}

                    <Input
                        id="description"
                        name="description"
                        type="text"
                        label="Description"
                        value={formData.description}
                        onChange={(value) => setFormData(prev => ({ ...prev, description: value }))}
                        placeholder="Enter task description"
                    />

                    <div className="row">
                        <div className="col-md-6">
                            <Dropdown
                                id="categoryId"
                                name="categoryId"
                                label="Category"
                                value={formData.categoryId}
                                options={categories.map(category => ({
                                    value: category.id,
                                    label: category.name
                                }))}
                                onChange={(value) => setFormData(prev => ({ ...prev, categoryId: String(value) }))}
                                placeholder="- Select a category -"
                                disabled={categoriesLoading}
                                allowEmpty={true}
                                required
                                containerClassName="mb-3"
                            />
                            {categoriesError && (
                                <div className="text-danger small">
                                    Failed to load categories
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md">
                            <DateTimePicker
                                id="from"
                                name="from"
                                label="Start Time"
                                value={formData.from}
                                onChange={(date) => setFormData(prev => ({ ...prev, from: date }))}
                                required
                                containerClassName="mb-3"
                            />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md">
                            <DateTimePicker
                                id="to"
                                name="to"
                                label="End Time"
                                value={formData.to}
                                onChange={(date) => setFormData(prev => ({ ...prev, to: date }))}
                                required
                                containerClassName="mb-3"
                            />
                        </div>
                    </div>
                </form>
            </ModalBody>

            <ModalFooter>
                <Button
                    type="button"
                    variant="secondary"
                    onClick={onClose}
                >
                    Cancel
                </Button>
                <Button
                    type="submit"
                    variant="primary"
                    onClick={handleSubmit}
                    disabled={timeEntryMutation.isPending}
                >
                    {timeEntryMutation.isPending 
                        ? 'Saving...' 
                        : (timeEntry ? 'Update' : 'Save')
                    }
                </Button>
            </ModalFooter>
        </Modal>
    );
};

export default TimeEntryPopup;