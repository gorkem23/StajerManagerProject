import React, { useEffect, useState } from 'react';
import type { Departman } from '../../api/services/departmanService'; // Error: Cannot find module
import type { Intern } from '../../../Stajers/api/types/stajer.types'; // Error: Cannot find module
import { api } from '@/utils/api'; // Error: Cannot find module
import DetailsStajerModal from '../../../Stajers/components/forms/DetailsStajerModal'; // Error: Cannot find module
import { Stajer } from '../../../Stajers/api/types/stajer.types'; // Error: Cannot find module


interface DetailsDepartmanModalProps {
	show: boolean;
	departman: Departman;
	onClose: () => void;
	onEdit?: () => void;
	isAdmin?: boolean;
}

export default function DetailsDepartmanModal({ show, departman, onClose, onEdit, isAdmin }: DetailsDepartmanModalProps) {
	const [interns, setInterns] = useState<Intern[]>([]);
	const [loading, setLoading] = useState(false);
	const [showDetailsModal, setShowDetailsModal] = useState(false);
	const [selectedStajer, setSelectedStajer] = useState<Stajer | null>(null);

	useEffect(() => {
		if (show && departman) {
			fetchInterns();
		} else {
			setInterns([]);
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [show, departman]);

	const fetchInterns = async () => {
		if (!departman) return;

		setLoading(true);
		try {
			const data = (await api
				.get('StajersApi', { searchParams: { sortBy: 'FullName', sortOrder: 'asc' } })
				.json()) as any;
			const list = Array.isArray(data?.stajers) ? data.stajers : Array.isArray(data) ? data : [];
			const filtered = list.filter((s: any) => s.departmanID === departman.departmanID);
			setInterns(
				filtered.map((s: any) => {

					const universiteAdi = typeof s.universite === 'string' 
						? s.universite 
						: s.universite?.universiteAdi || '';
					
					return {
						id: s.stajerID,
						adSoyad: s.fullName,
						email: s.email,
						universiteAdi: universiteAdi,
						stajer: s as Stajer
					};
				})
			);
		} catch (err) {
			console.error('Departman stajerleri yüklenemedi', err);
			setInterns([]);
		} finally {
			setLoading(false);
		}
	};

	const handleOpenInternDetailsModal = (internId: number) => {// *****  internId: number | string *****
		const intern = interns.find(i => i.id === internId || i.id === String(internId));
		if (intern?.stajer) {
			setSelectedStajer(intern.stajer);
			setShowDetailsModal(true);
		}
	};

	const handleCloseDetailsModal = () => {
		setShowDetailsModal(false);
		setSelectedStajer(null);
	};

	if (!departman) return null;

	return (
		<>
			<Dialog open={show} onClose={onClose} fullWidth maxWidth="sm">
				<DialogTitle>Departman Detayları</DialogTitle>
				<DialogContent className="flex flex-col gap-3 mt-2">
					<Typography variant="subtitle2" color="text.secondary">
						Departman Adı
					</Typography>
					<Typography variant="body1">{departman.departmanAdi || '-'}</Typography>

					<Typography variant="subtitle2" color="text.secondary" className="mt-4">
						Açıklama
					</Typography>
					<Typography variant="body1">{departman.aciklama || '-'}</Typography>

					<Divider className="my-2" />
					<Typography variant="subtitle1">İlgili Stajyerler</Typography>
					{loading ? (
						<Box className="flex justify-center py-4">
							<CircularProgress size={24} />
						</Box>
					) : interns.length === 0 ? (
						<Alert severity="info">Bu departmana ait kayıtlı stajyer yok.</Alert>
					) : (
						<Stack spacing={1}>
							{interns.map((intern) => (
								<Paper 
									key={intern.id} 
									className="p-2 flex justify-between items-center" 
									onClick={() => handleOpenInternDetailsModal(intern)}
									sx={{ cursor: 'pointer', '&:hover': { backgroundColor: 'action.hover' } }}
								>
									<div>
										<Typography variant="subtitle2">{intern.adSoyad || '-'}</Typography>
										<Typography variant="body2" color="text.secondary">{intern.email || '-'}</Typography>
									</div>
									<Chip label={intern.universiteAdi || 'Bilinmiyor'} size="small" />
								</Paper>
							))}
						</Stack>
					)}
				</DialogContent>
				<DialogActions>
					{isAdmin && onEdit && (
						<Button onClick={() => {onClose(); onEdit();}} variant="contained" color="warning">
							Düzenle
						</Button>
					)}
					<Button onClick={onClose}>Kapat</Button>
				</DialogActions>
			</Dialog>

			{selectedStajer && (
				<DetailsStajerModal
					show={showDetailsModal}
					stajer={selectedStajer}
					onClose={handleCloseDetailsModal}
				/>
			)}
		</>
	);
}
