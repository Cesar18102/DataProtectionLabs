#include <iostream>
#include <omp.h>

using namespace std;

long long INITIAL_PERMUTATION[64] = { 
	58, 50, 42, 34, 26, 18, 10, 2,
	60, 52, 44, 36, 28, 20, 12, 4,
	62, 54, 46, 38, 30, 22, 14, 6,
	64, 56, 48, 40, 32, 24, 16, 8,
	57, 49, 41, 33, 25, 17, 9, 1,
	59, 51, 43, 35, 27, 19, 11, 3,
	61, 53, 45, 37, 29, 21, 13, 5,
	63, 55, 47, 39, 31, 23, 15, 7
};

long long KEY_PERMUTATION[56] = {
	57, 49, 41, 33, 25, 17, 9, 1,
	58, 50, 42, 34, 26, 18, 10, 2, 
	59, 51, 43, 35, 27, 19, 11, 3, 
	60, 52, 44, 36, 63, 55, 47, 39, 
	31, 23, 15, 7, 62, 54, 46, 38, 
	30, 22, 14, 6, 61, 53, 45, 37, 
	29, 21, 13, 5, 28, 20, 12, 4
};

long long KEY_COMPRESSION[48] = {
	14, 17, 11, 24, 1, 5, 3, 28, 
	15, 6, 21, 10, 23, 19, 12, 4, 
	26, 8, 16, 7, 27, 20, 13, 2,
	41, 52, 31, 37, 47, 55, 30, 40, 
	51, 45, 33, 48, 44, 49, 39, 56, 
	34, 53, 46, 42, 50, 36, 29, 32
};

long long SHIFT_ROUNDS[16] = { 
	1, 1, 2, 2,
	2, 2, 2, 2,
	1, 2, 2, 2,
	2, 2, 2, 1 
};

long long D_BOX_PERMUTATION[48] = {
	32, 1, 2, 3, 4, 5, 4, 5,
	6, 7, 8, 9, 8, 9, 10, 11,
	12, 13, 12, 13, 14, 15, 16, 17,
	16, 17, 18, 19, 20, 21, 20, 21,
	22, 23, 24, 25, 24, 25, 26, 27,
	28, 29, 28, 29, 30, 31, 32, 1 
};

long long S_BOX_PERMUTATION[8][4][16] = {
	{ 
		14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
		0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
		4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
		15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 
	},
	{ 
		15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
		3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
		0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
		13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 
	},
	{ 
		10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
		13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
		13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
		1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 
	},
	{ 
		7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
		13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
		10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
		3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 
	},
	{ 
		2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
		14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
		4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
		11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 
	},
	{ 
		12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
		10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
		9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
		4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 
	},
	{ 
		4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
		13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
		1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
		6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 
	},
	{
		13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
		1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
		7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
		2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 
	} 
};

// Straight Permutation Table 
long long STRAIGHT_PERMUTATION[32] = {
	16, 7, 20, 21,
	29, 12, 28, 17,
	1, 15, 23, 26,
	5, 18, 31, 10,
	2, 8, 24, 14,
	32, 27, 3, 9,
	19, 13, 30, 6,
	22, 11, 4, 25 
};

long long FINAL_PERMUTATION[64] = {
	40, 8, 48, 16, 56, 24, 64, 32,
	39, 7, 47, 15, 55, 23, 63, 31,
	38, 6, 46, 14, 54, 22, 62, 30,
	37, 5, 45, 13, 53, 21, 61, 29,
	36, 4, 44, 12, 52, 20, 60, 28,
	35, 3, 43, 11, 51, 19, 59, 27,
	34, 2, 42, 10, 50, 18, 58, 26,
	33, 1, 41, 9, 49, 17, 57, 25 
};

long long FIRST_28_BITS =   0B1111111111111111111111111111000000000000000000000000000000000000;
long long LEFT_FIRST_BIT =  0B1000000000000000000000000000000000000000000000000000000000000000;
long long LEFT_LAST_BIT =   0B0000000000000000000000000001000000000000000000000000000000000000;

long long SECOND_28_BITS =  0B0000000000000000000000000000111111111111111111111111111100000000;
long long RIGHT_FIRST_BIT = 0B0000000000000000000000000000100000000000000000000000000000000000;
long long RIGHT_LAST_BIT =  0B0000000000000000000000000000000000000000000000000000000100000000;

long long FIRST_32_BITS =   0B1111111111111111111111111111111100000000000000000000000000000000;
long long LAST_32_BITS =    0B0000000000000000000000000000000011111111111111111111111111111111;

void print(long long x)
{
	for (int i = 0; i < 16; i++) {
		for (int j = 0; j < 4; j++)
			cout << ((x >> (64 - (i * 4 + j) - 1)) & 1);
		cout << " ";
	}
	cout << endl;
}

long long get_bit(long long x, int i, int max)
{
	return (x >> (max - i)) & 1;
}

long long permute(long long x, long long* permute_info, long long len, long long start, long long end)
{
	long long y = 0;
	for (int i = start; i < len; i++)
	{
		long long bit = get_bit(x, permute_info[i], end);
		long long shifted_back = bit << (end - i - 1);
		y |= shifted_back;
	}
	return y;
}

long long shift_cycle(long long x, long long first_bit_mask, long long last_bit_mask, long long mask) 
{
	long long bit = x & first_bit_mask;
	return ((x << 1) & mask) | ((bit >> 27) & last_bit_mask);
}

long long generate_next_key(long long &left_part, long long &right_part, int shift)
{
	for (int j = 0; j < shift; j++)
	{
		left_part = shift_cycle(left_part, LEFT_FIRST_BIT, LEFT_LAST_BIT, FIRST_28_BITS);
		right_part = shift_cycle(right_part, RIGHT_FIRST_BIT, RIGHT_LAST_BIT, SECOND_28_BITS);
	}

	return permute(left_part | right_part, KEY_COMPRESSION, 48, 0, 64);;
}

long long* generate_keys(long long &left_part_init, long long &right_part_init, void (*print)(int round, long long left, long long right, long long key))
{
	long long* keys = new long long[16];

	for (int i = 0; i < 16; i++)
	{
		keys[i] = generate_next_key(left_part_init, right_part_init, SHIFT_ROUNDS[i]);
		//print(i + 1, left_part_init, right_part_init, keys[i]);
	}

	return keys;
}

long long encrypt(long long text, long long* keys)
{
	long long text_permutated = permute(text, INITIAL_PERMUTATION, 64, 0, 64);
	//cout << "INIT TEXT PERMUTATION: ";
	//print(text_permutated);

	long long left_part = text_permutated & FIRST_32_BITS;
	//cout << "LEFT TEXT PART: ";
	//print(left_part);

	long long right_part = (text_permutated & LAST_32_BITS) << 32;
	//cout << "RIGHT TEXT PART: ";
	//print(right_part);

	for (int i = 0; i < 16; i++)
	{
		//cout << "\n*** KEY #" << i + 1 << " ***\n\n";

		long long right_expanded = permute(right_part, D_BOX_PERMUTATION, 48, 0, 64);
		//cout << "EXPANDED: ";
		//print(right_expanded);

		long long right_xored = keys[i] ^ right_expanded;
		//cout << "XORED: ";
		//print(right_xored);

		long long tabled = 0;
		for (int j = 0; j < 8; j++)
		{
			int row =  2 * get_bit(right_xored, j * 6 + 1, 64) +     get_bit(right_xored, j * 6 + 6, 64);
			int cell = 8 * get_bit(right_xored, j * 6 + 2, 64) + 4 * get_bit(right_xored, j * 6 + 3, 64) +
					   2 * get_bit(right_xored, j * 6 + 4, 64) +     get_bit(right_xored, j * 6 + 5, 64);

			long long val = S_BOX_PERMUTATION[j][row][cell];

			tabled |= (val / 8) << (64 - j * 4 - 1);
			val %= 8;

			tabled |= (val / 4) << (64 - j * 4 - 2);
			val %= 4;

			tabled |= (val / 2) << (64 - j * 4 - 3);
			val %= 2;

			tabled |= val << (64 - j * 4 - 4);
		}

		//cout << "TABLED: ";
		//print(tabled);

		long long tabled_permuted = permute(tabled, STRAIGHT_PERMUTATION, 32, 0, 64);
		//cout << "TABLED PERMUTED: ";
		//print(tabled_permuted);

		left_part = tabled_permuted ^ left_part;
		//cout << "TABLED PERMUTED XORED: ";
		//print(left_part);

		if (i != 15) {
			long long temp = left_part;
			left_part = right_part;
			right_part = temp;
		}

		long long entropy_text = left_part | ((right_part >> 32)& LAST_32_BITS);
		int ones_count = 0;

		for (int i = 1; i <= 64; i++)
			ones_count += get_bit(entropy_text, i, 64);

		//cout << "ENTROPY: " << ones_count << endl;
		//cout << "\n***********************\n\n";
	}

	return permute(left_part | ((right_part >> 32) & LAST_32_BITS), FINAL_PERMUTATION, 64, 0, 64);
}

void swap_vars(long long& a, long long& b)
{
	long long temp = a;
	a = b;
	b = temp;
}

void reverse_array(long long* arr, int n)
{
	for (int i = 0; i < n / 2; i++)
		swap_vars(arr[i], arr[n - i - 1]);
}

int main()
{
	double keygen_start = omp_get_wtime();

	long long init_key = 453;
	//cout << "INIT_KEY = ";
	//print(init_key);

	long long key = permute(init_key, KEY_PERMUTATION, 56, 0, 64);
	//cout << "KEY = ";
	//print(key);

	long long left_part = key & FIRST_28_BITS;
	//cout << "INIT LEFT PART = ";
	//print(left_part);

	long long right_part = key & SECOND_28_BITS;
	//cout << "INIT RIGHT PART = ";
	//print(right_part);

	long long* keys = generate_keys(
		left_part, 
		right_part,
		[](int round, long long left, long long right, long long key) {
			//cout << "\n*** ROUND " << round << " ***\n";

			//cout << "LEFT PART = ";
			//print(left);

			//cout << "RIGHT PART = ";
			//print(right);

			//cout << "KEY = ";
			//print(key);

			//cout << "***************\n";
		}
	);

	cout << "KEY GENERATION TOOK " << (omp_get_wtime() - keygen_start) * 1000000 << " micro sec.\n";
	cout << "\n\n****************************************\n\n";

	long long init_text = 123;

	double encrypt_start = omp_get_wtime();
	long long encrypted_text = encrypt(init_text, keys);

	cout << "ENCRYPTION TOOK " << (omp_get_wtime() - encrypt_start) * 1000000 << " micro sec.\n";
	cout << "ENCRYPTED TEXT = ";
	print(encrypted_text);

	cout << "\n\n****************************************\n\n";

	reverse_array(keys, 16);

	double decrypt_start = omp_get_wtime();
	long long decrypted_text = encrypt(encrypted_text, keys);

	cout << "DECRYPTION TOOK " << (omp_get_wtime() - decrypt_start) * 1000000 << " micro sec.\n";
	cout << "DECRYPTED TEXT = ";
	print(decrypted_text);
}