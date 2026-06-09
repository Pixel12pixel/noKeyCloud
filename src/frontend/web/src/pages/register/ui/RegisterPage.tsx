import {useEffect} from "react";
import {RegisterForm} from "@/widgets/register-form";
import {type ActionFunctionArgs, redirect, useActionData, useNavigation} from "react-router-dom";
import {generateSrpVerifier} from "@/shared/security";
import {backendBaseUrl} from "@/shared/config";

export async function registerAction({request}: ActionFunctionArgs) {
    const formData = await request.formData();
    const username = formData.get("username") as string;
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;
    const confirmPassword = formData.get("confirm-password") as string;
    const url = new URL(request.url);
    const registerInviteCode = url.searchParams.get("code");

    if (!registerInviteCode) {
        return {error: {errors: {body: ["An invite code is required to register."]}}};
    }

    if (!username && !email || !password) {
        return {error: {errors: {body: ["Username or email and password are required"]}}};
    }

    if (password !== confirmPassword) {
        return {error: {errors: {body: ["Passwords don't match"]}}};
    }

    if (password.length < 12) {
        return {error: {errors: {body: ["Password must be at least 12 characters long"]}}};
    }

    try {
        const authData = await generateSrpVerifier(username || email, password);

        const response = await fetch(`${backendBaseUrl}/api/Authenticate/register`, {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                username,
                email,
                salt: authData.saltBase64,
                verifier: authData.verifierBase64,
                registerInviteCode: registerInviteCode
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            return {error: {errors: {body: [errorText || "Invalid invite code or registration failed."]}}};
        }


        return redirect(`/login`);
    } catch (error: unknown) {
        const errorMessage = error instanceof Error ? error.message : "Failed to create account";
        return {error: {errors: {body: [errorMessage]}}};
    }
}

export function RegisterPage() {
    const actionData = useActionData<typeof registerAction>();
    const navigation = useNavigation();
    const isSubmitting = navigation.state === "submitting";
    const errorMessage = actionData?.error?.errors?.body?.[0];

    useEffect(() => {
        document.title = "Register - noKeyCloud";
    }, []);

    return (
        <div className="flex flex-1 w-full items-center justify-center p-6 md:p-10">
            <div className="w-full max-w-sm">
                <RegisterForm error={errorMessage} isSubmitting={isSubmitting}/>
            </div>
        </div>
    )
}