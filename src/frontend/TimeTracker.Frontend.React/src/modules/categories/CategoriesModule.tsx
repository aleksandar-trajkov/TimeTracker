import React, { useState } from 'react';
import { useCategoriesQuery, useCategoryMutation, useDeleteCategoryMutation } from '../../apiCalls/categories';
import type { CategoryMutationData } from '../../apiCalls/categories';
import { Button, Input } from '../../components/common';
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../components/modal';
import useUserStore from '../../stores/userStore';
import type { Category } from '../../types/Category';

interface CategoryFormData {
    name: string;
    description: string;
}

const CategoriesModule: React.FC = () => {
    const organizationId = useUserStore(state => state.user?.organizationId) || '';
    const { data: categories = [], isLoading, isError, error } = useCategoriesQuery(organizationId);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingCategory, setEditingCategory] = useState<Category | null>(null);
    const [formData, setFormData] = useState<CategoryFormData>({ name: '', description: '' });
    const [validationError, setValidationError] = useState<string | null>(null);

    const categoryMutation = useCategoryMutation({
        onSuccess: () => {
            setIsModalOpen(false);
            setEditingCategory(null);
            setFormData({ name: '', description: '' });
        }
    });

    const deleteMutation = useDeleteCategoryMutation();

    const handleAddNew = () => {
        setEditingCategory(null);
        setFormData({ name: '', description: '' });
        setValidationError(null);
        setIsModalOpen(true);
    };

    const handleEdit = (category: Category) => {
        setEditingCategory(category);
        setFormData({ name: category.name, description: category.description || '' });
        setValidationError(null);
        setIsModalOpen(true);
    };

    const handleDelete = (id: string) => {
        if (window.confirm('Are you sure you want to delete this category?')) {
            deleteMutation.mutate(id);
        }
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        setEditingCategory(null);
        setFormData({ name: '', description: '' });
        setValidationError(null);
    };

    const handleSubmit = (e?: React.FormEvent) => {
        if (e) e.preventDefault();

        if (!formData.name.trim()) {
            setValidationError('Category name is required.');
            return;
        }

        setValidationError(null);

        const mutationData: CategoryMutationData = {
            id: editingCategory?.id,
            name: formData.name.trim(),
            description: formData.description.trim() || undefined,
        };

        categoryMutation.mutate(mutationData);
    };

    if (isLoading) {
        return (
            <div className="d-flex justify-content-center mt-5">
                <div className="spinner-border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="alert alert-danger m-3" role="alert">
                <h4 className="alert-heading">Error loading categories</h4>
                <p>{(error as Error)?.message || 'Failed to load categories. Please try again.'}</p>
            </div>
        );
    }

    return (
        <React.Fragment>
            <div className="container-fluid">
                <div className="row">
                    <div className="col-12 d-flex justify-content-between align-items-center">
                        <h1>Categories</h1>
                        <Button type="button" variant="primary" onClick={handleAddNew}>
                            + New Category
                        </Button>
                    </div>
                </div>
                <div className="row mt-3">
                    <div className="col-12">
                        {categories.length === 0 ? (
                            <div className="text-center py-5">
                                <h3 className="text-muted">No categories found</h3>
                                <p className="text-muted">Create a category to organise your time entries.</p>
                            </div>
                        ) : (
                            <table className="table table-striped table-hover">
                                <thead className="table-dark">
                                    <tr>
                                        <th>Name</th>
                                        <th>Description</th>
                                        <th style={{ width: '140px' }}>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {categories.map((category) => (
                                        <tr key={category.id}>
                                            <td>{category.name}</td>
                                            <td>{category.description || '—'}</td>
                                            <td>
                                                <Button
                                                    type="button"
                                                    variant="secondary"
                                                    onClick={() => handleEdit(category)}
                                                >
                                                    Edit
                                                </Button>
                                                {' '}
                                                <Button
                                                    type="button"
                                                    variant="danger"
                                                    onClick={() => handleDelete(category.id)}
                                                    disabled={deleteMutation.isPending}
                                                >
                                                    Delete
                                                </Button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        )}
                    </div>
                </div>
            </div>

            <Modal isOpen={isModalOpen} onClose={handleModalClose} size="md">
                <ModalHeader
                    title={editingCategory ? 'Edit Category' : 'New Category'}
                    onClose={handleModalClose}
                />
                <ModalBody>
                    <form onSubmit={handleSubmit}>
                        {validationError && (
                            <div className="alert alert-warning py-2" role="alert">
                                {validationError}
                            </div>
                        )}
                        <Input
                            id="category-name"
                            name="name"
                            type="text"
                            label="Name"
                            value={formData.name}
                            onChange={(value) => setFormData(prev => ({ ...prev, name: value }))}
                            placeholder="Enter category name"
                            required
                        />
                        <Input
                            id="category-description"
                            name="description"
                            type="text"
                            label="Description"
                            value={formData.description}
                            onChange={(value) => setFormData(prev => ({ ...prev, description: value }))}
                            placeholder="Enter optional description"
                        />
                    </form>
                </ModalBody>
                <ModalFooter>
                    <Button type="button" variant="secondary" onClick={handleModalClose}>
                        Cancel
                    </Button>
                    <Button
                        type="submit"
                        variant="primary"
                        onClick={handleSubmit}
                        disabled={categoryMutation.isPending}
                    >
                        {categoryMutation.isPending ? 'Saving...' : (editingCategory ? 'Update' : 'Save')}
                    </Button>
                </ModalFooter>
            </Modal>
        </React.Fragment>
    );
};

export default CategoriesModule;