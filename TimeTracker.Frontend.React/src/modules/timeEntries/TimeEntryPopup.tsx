import React, { useState } from 'react';
import type { TimeEntry, Category } from '../../apiCalls/timeEntries';
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../components/modal';
import { Input, Button } from '../../components/common';
import { DateTimePicker } from '../../components/date';

interface TimeEntryPopupProps {
    onClose: () => void;
    timeEntry?: TimeEntry | null;
}

interface FormData {
    description: string;
    from: Date | null;
    to: Date | null;
    categoryName: string;
}

const TimeEntryPopup: React.FC<TimeEntryPopupProps> = ({ onClose, timeEntry = null }) => {
    const [formData, setFormData] = useState<FormData>(() => ({
        description: timeEntry?.description || '',
        from: timeEntry?.from || null,
        to: timeEntry?.to || null,
        categoryName: timeEntry?.category?.name || '',
    }));

    const handleSubmit = (e?: React.FormEvent) => {
        if (e) {
            e.preventDefault();
        }

        // Create category object if category name is provided
        const category: Category | null = formData.categoryName
            ? { id: timeEntry?.category?.id || 0, name: formData.categoryName }
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
                        required
                    />

                    <div className="row">
                        <div className="col-md-6">
                            <Input
                                id="categoryName"
                                name="categoryName"
                                type="text"
                                label="Category"
                                value={formData.categoryName}
                                onChange={(value) => setFormData(prev => ({ ...prev, categoryName: value }))}
                                placeholder="Category name"
                                containerClassName="mb-3"
                            />
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