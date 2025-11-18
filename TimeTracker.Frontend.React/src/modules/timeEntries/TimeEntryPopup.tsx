import React, { useState } from 'react';
import type { TimeEntry } from '../../types/TimeEntry';
import type { Category } from '../../types/Category';
import { useCategoriesQuery } from '../../apiCalls/categories';
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../components/modal';
import { Input, Button, Dropdown } from '../../components/common';
import { DateTimePicker } from '../../components/date';
import useUserStore from '../../stores/userStore';

interface TimeEntryPopupProps {
    onClose: () => void;
    timeEntry?: TimeEntry | null;
}

interface FormData {
    description: string;
    from: Date | null;
    to: Date | null;
    categoryId: string;
}

const TimeEntryPopup: React.FC<TimeEntryPopupProps> = ({ onClose, timeEntry = null }) => {
    const organizationId = useUserStore(state => state.user?.organizationId);
    const { data: categories = [], isLoading: categoriesLoading, error: categoriesError } = useCategoriesQuery(organizationId || '');
    
    const [formData, setFormData] = useState<FormData>(() => ({
        description: timeEntry?.description || '',
        from: timeEntry?.from || null,
        to: timeEntry?.to || null,
        categoryId: timeEntry?.category?.id || '',
    }));

    const handleSubmit = (e?: React.FormEvent) => {
        if (e) {
            e.preventDefault();
        }

        // Find the complete category object from the loaded categories
        const category: Category | null = formData.categoryId
            ? categories.find(cat => cat.id === formData.categoryId) || null
            : null;

        const timeEntryData: Partial<TimeEntry> = {
            id: timeEntry?.id,
            description: formData.description,
            from: formData.from || new Date(),
            to: formData.to || new Date(),
            category,
        };

        // TODO: Implement save functionality
        console.log('Saving time entry:', timeEntryData);

        onClose();
    };

    if (!timeEntry) return null;

    return (
        <Modal 
            isOpen={true} 
            onClose={onClose} 
            size="md"
        >
            <ModalHeader
                title={timeEntry ? 'Edit Time Entry' : 'Add Time Entry'}
                onClose={onClose}
            />

            <ModalBody>
                <form onSubmit={handleSubmit}>
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
                >
                    {timeEntry ? 'Update' : 'Save'}
                </Button>
            </ModalFooter>
        </Modal>
    );
};

export default TimeEntryPopup;